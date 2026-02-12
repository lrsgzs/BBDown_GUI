using System;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BBDown.Core.Util;

namespace BBDown_GUI.Services;

public partial class BiliBiliVideoService
{
    [GeneratedRegex("av(\\d+)")]
    private static partial Regex AvRegex();
    
    [GeneratedRegex("[Bb][Vv]1(\\w+)")]
    private static partial Regex BvRegex();
    
    [GeneratedRegex("/ep(\\d+)")]
    private static partial Regex EpRegex();
    
    [GeneratedRegex("/ss(\\d+)")]
    private static partial Regex SsRegex();
    
    [GeneratedRegex(@"space\.bilibili\.com/(\d+)")]
    private static partial Regex UidRegex();
    
    [GeneratedRegex(@"\.bilibili\.tv\/\w+\/play\/\d+\/(\d+)")]
    private static partial Regex GlobalEpRegex();
    
    [GeneratedRegex("bangumi/media/(md\\d+)")]
    private static partial Regex BangumiMdRegex();
    
    [GeneratedRegex(@"window.__INITIAL_STATE__=([\s\S].*?);\(function\(\)")]
    private static partial Regex StateRegex();
    
    [GeneratedRegex("md(\\d+)")]
    private static partial Regex MdRegex();
    
    private static string GetAidByBv(string bv)
    {
        // 能在本地就在本地
        return BilibiliBvConverter.Decode(bv).ToString();
    }
    
    private static async Task<string> GetEpidBySsidAsync(string ssid)
    {
        var api = $"https://api.bilibili.com/pugv/view/web/season?season_id={ssid}";
        var json = await HTTPUtil.GetWebSourceAsync(api);
        using var jDoc = JsonDocument.Parse(json);
        var epId = jDoc.RootElement.GetProperty("data").GetProperty("episodes").EnumerateArray().First().GetProperty("id").ToString();
        return epId;
    }
    
    private static async Task<string> GetEpidByBangumiSsidAsync(string ssId)
    {
        var api = $"https://{BBDown.Core.Config.EPHOST}/pgc/view/web/season?season_id={ssId}";
        var json = await HTTPUtil.GetWebSourceAsync(api);
        using var jDoc = JsonDocument.Parse(json);
        var epId = jDoc.RootElement.GetProperty("result").GetProperty("episodes").EnumerateArray().First().GetProperty("id").ToString();
        return epId;
    }
    
    private static async Task<string> GetEpidByMdAsync(string mdid)
    {
        var api = $"https://api.bilibili.com/pgc/review/user?media_id={mdid}";
        var json = await HTTPUtil.GetWebSourceAsync(api);
        using var jDoc = JsonDocument.Parse(json);
        var epId = jDoc.RootElement.GetProperty("result").GetProperty("media").GetProperty("new_ep").GetProperty("id").ToString();
        return epId;
    }
    
    /// <summary>
    /// 通过avid检测是否为版权内容, 如果是的话返回ep:xx格式
    /// </summary>
    /// <param name="avid"></param>
    /// <returns></returns>
    private static async Task<string> FixAvidAsync(string avid)
    {
        if (!avid.All(char.IsDigit))
            return avid;
        var api = $"https://www.bilibili.com/video/av{avid}/";
        var location = await HTTPUtil.GetWebLocationAsync(api);
        return location.Contains("/ep") ? $"ep:{EpRegex().Match(location).Groups[1].Value}" : avid;
    }
    
    public static async Task<string> GetAvIdAsync(string input)
    {
        var avid = input;
        if (input.StartsWith("http"))
        {
            if (input.Contains("b23.tv"))
            {
                var tmp = await HTTPUtil.GetWebLocationAsync(input);
                if (tmp == input) throw new Exception("无限重定向");
                input = tmp;
            }
            if (input.Contains("video/av"))
            {
                avid = AvRegex().Match(input).Groups[1].Value;
            }
            else if (input.ToLower().Contains("video/bv"))
            {
                avid = GetAidByBv(BvRegex().Match(input).Groups[1].Value);
            }
            else if (input.Contains("/cheese/"))
            {
                var epId = "";
                if (input.Contains("/ep"))
                {
                    epId = EpRegex().Match(input).Groups[1].Value;
                }
                else if (input.Contains("/ss"))
                {
                    epId = await GetEpidBySsidAsync(SsRegex().Match(input).Groups[1].Value);
                }
                avid = $"cheese:{epId}";
            }
            else if (input.Contains("/ep"))
            {
                var epId = EpRegex().Match(input).Groups[1].Value;
                avid = $"ep:{epId}";
            }
            else if (input.Contains("/ss"))
            {
                var epId = await GetEpidByBangumiSsidAsync(SsRegex().Match(input).Groups[1].Value);
                avid = $"ep:{epId}";
            }
            else if (input.Contains("/medialist/") && input.Contains("business_id=") && input.Contains("business=space_collection")) // 列表类型是合集
            {
                var bizId = Utils.GetQueryString("business_id", input);
                avid = $"listBizId:{bizId}";
            }
            else if (input.Contains("/medialist/") && input.Contains("business_id=") && input.Contains("business=space_series")) // 列表类型是系列
            {
                var bizId = Utils.GetQueryString("business_id", input);
                avid = $"seriesBizId:{bizId}";
            }
            else if (input.Contains("/channel/collectiondetail?sid="))
            {
                var bizId = Utils.GetQueryString("sid", input);
                avid = $"listBizId:{bizId}";
            }
            else if (input.Contains("/channel/seriesdetail?sid="))
            {
                var bizId = Utils.GetQueryString("sid", input);
                avid = $"seriesBizId:{bizId}";
            }
            // 新版个人空间合集/系列链接兼容：
            // 例如：
            //   合集: https://space.bilibili.com/392959666/lists/1560264?type=season
            //   系列: https://space.bilibili.com/392959666/lists/1560264?type=series
            else if (input.Contains("/space.bilibili.com/") && input.Contains("/lists/"))
            {
                var type = Utils.GetQueryString("type", input).ToLower();
                // path 最后一个 / 后到 ? 前即为 sid
                var path = input.Split('?', '#')[0];
                var sidPart = path[(path.LastIndexOf('/') + 1)..];

                if (type == "season")
                {
                    avid = $"listBizId:{sidPart}";
                }
                else if (type == "series")
                {
                    avid = $"seriesBizId:{sidPart}";
                }
                else
                {
                    // 未知类型按合集处理，至少不会识别失败
                    avid = $"listBizId:{sidPart}";
                }
            }
            else if (input.Contains("/space.bilibili.com/") && input.Contains("/favlist"))
            {
                var mid = UidRegex().Match(input).Groups[1].Value;
                var fid = Utils.GetQueryString("fid", input);
                avid = $"favId:{fid}:{mid}";
            }
            else if (input.Contains("/space.bilibili.com/"))
            {
                var mid = UidRegex().Match(input).Groups[1].Value;
                avid = $"mid:{mid}";
            }
            else if (input.Contains("ep_id="))
            {
                var epId = Utils.GetQueryString("ep_id", input);
                avid = $"ep:{epId}";
            }
            else if (GlobalEpRegex().Match(input).Success)
            {
                var epId = GlobalEpRegex().Match(input).Groups[1].Value;
                avid = $"ep:{epId}";
            }
            else if (BangumiMdRegex().Match(input).Success)
            {
                var mdId = BangumiMdRegex().Match(input).Groups[1].Value;
                var epId = await GetEpidByMdAsync(mdId);
                avid = $"ep:{epId}";
            }
            else
            {
                string web = await HTTPUtil.GetWebSourceAsync(input);
                var regex = StateRegex();
                var json = regex.Match(web).Groups[1].Value;
                using var jDoc = JsonDocument.Parse(json);
                var epId = jDoc.RootElement.GetProperty("epList").EnumerateArray().First().GetProperty("id").ToString();
                avid = $"ep:{epId}";
            }
        }
        else if (input.ToLower().StartsWith("bv"))
        {
            avid = GetAidByBv(input[3..]);
        }
        else if (input.ToLower().StartsWith("av")) // av
        {
            avid = input.ToLower()[2..];
        }
        else if (input.StartsWith("cheese/")) // ^cheese/(ep|ss)\d+ 格式
        {
            var epId = "";
            if (input.Contains("/ep"))
            {
                epId = EpRegex().Match(input).Groups[1].Value;
            }
            else if (input.Contains("/ss"))
            {
                epId = await GetEpidBySsidAsync(SsRegex().Match(input).Groups[1].Value);
            }
            avid = $"cheese:{epId}";
        }
        else if (input.StartsWith("ep"))
        {
            var epId = input[2..];
            avid = $"ep:{epId}";
        }
        else if (input.StartsWith("ss"))
        {
            var epId = await GetEpidByBangumiSsidAsync(input[2..]);
            avid = $"ep:{epId}";
        }
        else if (input.StartsWith("md"))
        {
            var mdId = MdRegex().Match(input).Groups[1].Value;
            var epId = await GetEpidByMdAsync(mdId);
            avid = $"ep:{epId}";
        }
        else
        {
            throw new Exception("输入有误");
        }
        return await FixAvidAsync(avid);
    }
}