using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using BBDown.Core.Util;

namespace BBDown_GUI;

public static partial class Utils
{
    [GeneratedRegex("(^|&)?(\\w+)=([^&]+)(&|$)?", RegexOptions.Compiled)]
    private static partial Regex QueryRegex();
    
    private static readonly Random Random = new();
    
    /// <summary>
    /// 获取url字符串参数, 返回参数值字符串
    /// </summary>
    /// <param name="name">参数名称</param>
    /// <param name="url">url字符串</param>
    /// <returns></returns>
    public static string GetQueryString(string name, string url)
    {
        var re = QueryRegex();
        var mc = re.Matches(url);
        foreach (var match in mc.Cast<Match>())
        {
            if (match.Result("$2").Equals(name))
            {
                return match.Result("$3");
            }
        }
        
        return "";
    }

    public static string RSubString(string sub)
    {
        sub = sub[(sub.LastIndexOf('/') + 1)..];
        return sub[..sub.LastIndexOf('.')];
    }

    public static string GetMixinKey(string orig)
    {
        byte[] mixinKeyEncTab = 
        [
            46, 47, 18, 2, 53, 8, 23, 32, 15, 50, 10, 31, 58, 3, 45, 35,
            27, 43, 5, 49, 33, 9, 42, 19, 29, 28, 14, 39, 12, 38, 41, 13
        ];

        var tmp = new StringBuilder(32);
        foreach (var index in mixinKeyEncTab)
        {
            tmp.Append(orig[index]);
        }
        return tmp.ToString();
    }
    
    public static string GetSign(string parms)
    {
        var toEncode = parms + "59b43e04ad6965f34319062b478f83dd";
        return string.Concat(MD5.HashData(Encoding.UTF8.GetBytes(toEncode)).Select(i => i.ToString("x2")));
    }

    public static string GetTimeStamp(bool bflag)
    {
        var ts = DateTimeOffset.Now;
        return (bflag ? ts.ToUnixTimeSeconds() : ts.ToUnixTimeMilliseconds()).ToString();
    }

    public static string GetRandomString(int length)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
    
    public static string ToQueryString(NameValueCollection nameValueCollection)
    {
        var httpValueCollection = HttpUtility.ParseQueryString(string.Empty);
        httpValueCollection.Add(nameValueCollection);
        return httpValueCollection.ToString()!;
    }
    
    public static Dictionary<string, string> ToDictionary(this NameValueCollection nameValueCollection)
    {
        var dict = new Dictionary<string, string>();
        foreach (var key in nameValueCollection.AllKeys)
        {
            dict[key!] = nameValueCollection[key]!;
        }
        return dict;
    }
    
    public static string FormatTime(int time, bool absolute = false)
    {
        var ts = TimeSpan.FromSeconds(time);
        var totalHours = (int)ts.TotalHours;
        var minutes = ts.Minutes;
        var seconds = ts.Seconds;

        if (absolute)
        {
            return $"{totalHours:D2}:{minutes:D2}:{seconds:D2}";
        }

        return totalHours == 0 ? $"{minutes:D2}m{seconds:D2}s" : $"{totalHours}h{minutes:D2}m{seconds:D2}s";
    }
    
    public static string GetFilePath(params string[] strings)
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ?? "BBDown_GUI";
        var path = Path.Combine([appData, appName, ..strings]);
        
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        return path;
    }
}