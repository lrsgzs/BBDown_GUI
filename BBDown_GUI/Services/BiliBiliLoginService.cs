using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BBDown_GUI.Models;
using BBDown_GUI.Services.Config;
using BBDown.Core.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using QRCoder;

namespace BBDown_GUI.Services;

public partial class BiliBiliLoginService : ObservableRecipient
{
    [ObservableProperty] private Api.X.WebInterface.NavData _lastLoginData = new();
    
    private ILogger<BiliBiliLoginService> Logger { get; }
    private ConfigHandler ConfigHandler { get; }

    public BiliBiliLoginService(ILogger<BiliBiliLoginService> logger, ConfigHandler configHandler)
    {
        Logger = logger;
        ConfigHandler = configHandler;

        if (ConfigHandler.Data.WebCookie != string.Empty)
        {
            // 防止 BBDown.Core 崩掉
            _ = CheckWebLogin();
            _ = CheckTvLogin();
        }
    }
    
    public async Task<string> GetWebLoginStatusAsync(string qrcodeKey)
    {
        var queryUrl = $"https://passport.bilibili.com/x/passport-login/web/qrcode/poll?qrcode_key={qrcodeKey}&source=main-fe-header";
        return await HTTPUtil.GetWebSourceAsync(queryUrl);
    }

    public async Task<bool> CheckWebLogin()
    {
        BBDown.Core.Config.COOKIE = ConfigHandler.Data.WebCookie;
        try
        {
            var source = await HTTPUtil.GetWebSourceAsync("https://api.bilibili.com/x/web-interface/nav");
            var data = JsonSerializer.Deserialize<BaseResponse<Api.X.WebInterface.NavData>>(source)!;
            if (!data.TryGetData(out var navData)) return false;
            LastLoginData = navData;
            
            var isLogin = navData.IsLogin;
            var wbiImg = navData.WbiImg;
            BBDown.Core.Config.WBI = Utils.GetMixinKey(Utils.RSubString(wbiImg.ImgUrl) + Utils.RSubString(wbiImg.SubUrl));
            Logger.LogDebug("wbi: {WBI}", BBDown.Core.Config.WBI);
            return isLogin;
        }
        catch (Exception e)
        {
            return false;
        }
    }
    
    public async Task<bool> CheckTvLogin()
    {
        BBDown.Core.Config.TOKEN = ConfigHandler.Data.TvToken;
        return ConfigHandler.Data.TvToken != string.Empty;
    }
    
    public async Task<WebLoginData> GenerateWebLoginData()
    {
        Logger.LogInformation("获取 Web 版登录地址...");
        const string loginUrl = "https://passport.bilibili.com/x/passport-login/web/qrcode/generate?source=main-fe-header";
        var url = JsonDocument.Parse(await HTTPUtil.GetWebSourceAsync(loginUrl))
            .RootElement.GetProperty("data").GetProperty("url").ToString();
        var qrCodeKey = Utils.GetQueryString("qrcode_key", url);

        Logger.LogInformation("生成二维码...");
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
        var pngByteCode = new PngByteQRCode(qrCodeData);

        var qrCodePath = Utils.GetFilePath("Cache", "qrcode.png");
        await File.WriteAllBytesAsync(qrCodePath, pngByteCode.GetGraphic(7));

        Logger.LogInformation("已生成二维码到 {PATH}", qrCodePath);
        return new WebLoginData(qrCodePath, qrCodeKey);
    }

    public async IAsyncEnumerable<LoginStatus> WaitLoginWeb(WebLoginData data)
    {
        var isScanned = false;
        var isFinished = false;
        
        while (!isFinished)
        {
            await Task.Delay(1000);
            
            var w = await GetWebLoginStatusAsync(data.QrCodeKey);
            var code = JsonDocument.Parse(w).RootElement.GetProperty("data").GetProperty("code").GetInt32();
            switch (code)
            {
                case 86038:  // 已过期
                    Logger.LogInformation("二维码已过期, 需重新登录");
                    yield return new LoginStatus(false, code, "二维码已过期, 请重新登录！");
                    isFinished = true;
                    break;
                case 86101:  // 等待扫码
                    yield return new LoginStatus(false, code, "等待扫码...");
                    break;
                case 86090:  // 等待确认
                {
                    if (!isScanned)
                    {
                        Logger.LogInformation("扫码成功，还未确认");
                        yield return new LoginStatus(false, code, "扫码成功, 请确认...");
                        isScanned = !isScanned;
                    }
                    break;
                }
                default:
                {
                    var cc = JsonDocument.Parse(w).RootElement.GetProperty("data").GetProperty("url").ToString();
                    Logger.LogInformation("登录成功！SESSDATA={DATA}", Utils.GetQueryString("SESSDATA", cc));
                    
                    // 导出cookie, 转义英文逗号 否则部分场景会出问题
                    ConfigHandler.Data.WebCookie = cc[(cc.IndexOf('?') + 1)..].Replace("&", ";").Replace(",", "%2C");
                    File.Delete(data.QrCodePath);
                    
                    // 防止 BBDown.Core 崩掉
                    await CheckWebLogin();
                    
                    yield return new LoginStatus(true, 0, "登录成功！");
                    isFinished = true;
                    break;
                }
            }
        }
    }

    public static NameValueCollection GetTvLoginParams()
    {
        var tvParams = new NameValueCollection();
        var now = DateTime.Now;
        var deviceId = Utils.GetRandomString(20);
        var buvid = Utils.GetRandomString(37);
        var fingerprint = $"{now:yyyyMMddHHmmssfff}{Utils.GetRandomString(45)}";
        
        tvParams.Add("appkey", "4409e2ce8ffd12b8");
        tvParams.Add("auth_code", "");
        tvParams.Add("bili_local_id", deviceId);
        tvParams.Add("build", "102801");
        tvParams.Add("buvid", buvid);
        tvParams.Add("channel", "master");
        tvParams.Add("device", "OnePlus");
        tvParams.Add("device_id", deviceId);
        tvParams.Add("device_name", "OnePlus7TPro");
        tvParams.Add("device_platform", "Android10OnePlusHD1910");
        tvParams.Add("fingerprint", fingerprint);
        tvParams.Add("guid", buvid);
        tvParams.Add("local_fingerprint", fingerprint);
        tvParams.Add("local_id", buvid);
        tvParams.Add("mobi_app", "android_tv_yst");
        tvParams.Add("networkstate", "wifi");
        tvParams.Add("platform", "android");
        tvParams.Add("sys_ver", "29");
        tvParams.Add("ts", Utils.GetTimeStamp(true));
        tvParams.Add("sign", Utils.GetSign(Utils.ToQueryString(tvParams)));

        return tvParams;
    }
    
    public async Task<TvLoginData> GenerateTvLoginData()
    {
        var tvParams = GetTvLoginParams();
        
        Logger.LogInformation("获取 TV 版登录地址...");
        var response = await HTTPUtil.AppHttpClient.PostAsync(
            "https://passport.snm0516.aisee.tv/x/passport-tv-login/qrcode/auth_code",
            new FormUrlEncodedContent(tvParams.ToDictionary()));
        var responseArray = await response.Content.ReadAsByteArrayAsync();
        var web = Encoding.UTF8.GetString(responseArray);
        var url = JsonDocument.Parse(web).RootElement.GetProperty("data").GetProperty("url").ToString();
        var authCode = JsonDocument.Parse(web).RootElement.GetProperty("data").GetProperty("auth_code").ToString();
        
        Logger.LogInformation("生成二维码...");
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
        var pngByteCode = new PngByteQRCode(qrCodeData);
        
        var qrCodePath = Utils.GetFilePath("Cache", "qrcode.png");
        await File.WriteAllBytesAsync(qrCodePath, pngByteCode.GetGraphic(7));
        
        Logger.LogInformation("已生成二维码到 {PATH}", qrCodePath);
        
        tvParams.Set("auth_code", authCode);
        tvParams.Set("ts", Utils.GetTimeStamp(true));
        tvParams.Remove("sign");
        tvParams.Add("sign", Utils.GetSign(Utils.ToQueryString(tvParams)));

        return new TvLoginData(tvParams, qrCodePath);
    }
    
    public async IAsyncEnumerable<LoginStatus> WaitLoginTv(TvLoginData data)
    {
        var isFinished = false;
        
        while (!isFinished)
        {
            await Task.Delay(1000);

            var response = await HTTPUtil.AppHttpClient.PostAsync(
                "https://passport.bilibili.com/x/passport-tv-login/qrcode/poll",
                new FormUrlEncodedContent(data.Params.ToDictionary()));
            var responseArray = await response.Content.ReadAsByteArrayAsync();
            var web = Encoding.UTF8.GetString(responseArray);
            
            var code = JsonDocument.Parse(web).RootElement.GetProperty("code").GetInt32();
            switch (code)
            {
                case 86038:  // 已过期
                    Logger.LogInformation("二维码已过期, 需重新登录");
                    yield return new LoginStatus(false, code, "二维码已过期, 请重新登录！");
                    isFinished = true;
                    break;
                case 86039:  // 等待扫码
                    yield return new LoginStatus(false, code, "等待扫码...");
                    break;
                default:
                {
                    var token = JsonDocument.Parse(web).RootElement.GetProperty("data").GetProperty("access_token").ToString();
                    Logger.LogInformation("登录成功！AccessToken=={TOKEN}", token);
                    ConfigHandler.Data.TvToken = token;
                    File.Delete(data.QrCodePath);
                    
                    yield return new LoginStatus(true, 0, "登录成功！");
                    isFinished = true;
                    break;
                }
            }
        }
    }
}