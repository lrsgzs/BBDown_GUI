using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BBDown_GUI;

public static partial class Utils
{
    [GeneratedRegex("(^|&)?(\\w+)=([^&]+)(&|$)?", RegexOptions.Compiled)]
    private static partial Regex QueryRegex();
    
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