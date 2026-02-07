using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using BBDown_GUI.Models;
using BBDown_GUI.Services.Config;
using BBDown.Core.Util;
using Microsoft.Extensions.Logging;
using QRCoder;

namespace BBDown_GUI.Services;

public class BiliBiliLoginService
{
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
        }
    }
    
    public async Task<string> GetLoginStatusAsync(string qrcodeKey)
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
            var json = JsonDocument.Parse(source).RootElement;
            var isLogin = json.GetProperty("data").GetProperty("isLogin").GetBoolean();
            var wbiImg = json.GetProperty("data").GetProperty("wbi_img");
            BBDown.Core.Config.WBI = Utils.GetMixinKey(Utils.RSubString(wbiImg.GetProperty("img_url").GetString()) + Utils.RSubString(wbiImg.GetProperty("sub_url").GetString()));
            Logger.LogDebug("wbi: {WBI}", BBDown.Core.Config.WBI);
            return isLogin;
        }
        catch (Exception e)
        {
            return false;
        }
    }
    
    public async Task<LoginData> GenerateWebLoginData()
    {
        Logger.LogInformation("获取登录地址...");
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
        return new LoginData(qrCodePath, qrCodeKey);
    }

    public async IAsyncEnumerable<LoginStatus> WaitLoginWeb(LoginData data)
    {
        var isScanned = false;
        var isFinished = false;
        
        while (!isFinished)
        {
            await Task.Delay(1000);
            
            var w = await GetLoginStatusAsync(data.QrCodeKey);
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
                    continue;
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
}