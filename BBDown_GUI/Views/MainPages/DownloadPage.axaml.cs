using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using BBDown_GUI.Abstraction;
using BBDown_GUI.Attributes;
using BBDown_GUI.Enums;
using BBDown_GUI.Helpers.UI;
using BBDown_GUI.Models.Video;
using BBDown_GUI.Services;
using BBDown_GUI.Services.Config;
using BBDown_GUI.ViewModels.MainPages;
using BBDown.Core;
using BBDown.Core.Entity;
using DynamicData;
using Microsoft.Extensions.Logging;

namespace BBDown_GUI.Views.MainPages;

[MainPageInfo("下载视频", "download", "\uE0D3", true)]
public partial class DownloadPage : UserControl
{
    private ILogger<DownloadPage> Logger { get; } = IAppHost.GetService<ILogger<DownloadPage>>();
    public DownloadPageViewModel ViewModel { get; } = IAppHost.GetService<DownloadPageViewModel>();
    public ConfigHandler ConfigHandler { get; } = IAppHost.GetService<ConfigHandler>();
    public BiliBiliLoginService BiliBiliLoginService { get; } = IAppHost.GetService<BiliBiliLoginService>();
    public BiliBiliVideoService BiliBiliVideoService { get; } = IAppHost.GetService<BiliBiliVideoService>();
    
    public DownloadPage()
    {
        DataContext = this;
        InitializeComponent();
    }

    private async Task ProcessPageSwitch()
    {
        try
        {
            switch (ViewModel.PageIndex)
            {
                case 1:
                    await PullVideoInfo();
                    break;
                default:
                    break;
            }
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "在翻页时出现错误。当前页码：{PAGE}", ViewModel.PageIndex);
            this.ShowErrorToast("出错了", exception);
        }
    }
    
    private void PrevButton_OnClick(object? sender, RoutedEventArgs e)
    {
        SlideCarousel.Previous();
    }
    
    private void NextButton_OnClick(object? sender, RoutedEventArgs e)
    {
        SlideCarousel.Next();
        _ = ProcessPageSwitch();
    }

    private async Task PullVideoInfo()
    {
        ViewModel.VideoInfo = null;
        ViewModel.Pages.Clear();
        
        bool isLoggedIn;
        if (ViewModel.Mode == DownloadMode.Web)
        {
            isLoggedIn = await BiliBiliLoginService.CheckWebLogin();
        }
        else
        {
            isLoggedIn = await BiliBiliLoginService.CheckTvLogin();
        }
        Logger.LogInformation("登录状态：{STATUS}", isLoggedIn);

        if (!isLoggedIn)
        {
            this.ShowWarningToast("你尚未登录 B 站账号, 解析可能受到限制L");
        }
        
        var aidOri = await BiliBiliVideoService.GetAvIdAsync(ViewModel.VideoLink);
        Logger.LogInformation("获取到的aid: {AID_ORI}", aidOri);

        if (string.IsNullOrEmpty(aidOri))
        {
            this.ShowErrorToast("输入有误");
            SlideCarousel.Previous();
            return;
        }
        
        Logger.LogInformation("获取视频信息...");
        var fetcher = FetcherFactory.CreateFetcher(aidOri, ViewModel.UseIntlApi);
        VInfo? vInfo;
        
        // 只输入 EP/SS 时优先按番剧查找，如果找不到则尝试按课程查找
        try
        {
            vInfo = await fetcher.FetchAsync(aidOri);
        }
        catch (KeyNotFoundException e)
        {
            if (e.Message != "Arg_KeyNotFound") throw; // 错误消息不符合预期，抛出异常
            if (aidOri.StartsWith("cheese:")) throw; // 已经按课程查找过，不再重复尝试

            Logger.LogWarning("未找到此 EP/SS 对应番剧信息, 正在尝试按课程查找。");

            aidOri = aidOri.Replace("ep", "cheese");
            Logger.LogInformation("新的 aid: {AID_ORI}", aidOri);

            if (string.IsNullOrEmpty(aidOri))
            {
                throw new Exception("输入有误");
            }

            Logger.LogInformation("获取视频信息...");
            fetcher = FetcherFactory.CreateFetcher(aidOri, ViewModel.UseIntlApi);
            vInfo = await fetcher.FetchAsync(aidOri);
        }

        ViewModel.AidOri = aidOri;
        ViewModel.VideoInfo = vInfo;
        
        if (vInfo.IsSteinGate && ViewModel.Mode == DownloadMode.Tv)
        {
            Logger.LogWarning("视频为互动视频，暂时不支持 TV 下载，修改为默认下载");
            this.ShowWarningToast("视频为互动视频，暂时不支持 TV 下载，修改为默认下载");
            ViewModel.Mode = DownloadMode.Web;
        }

        using (Logger.BeginScope("分 P 信息"))
        {
            var more = false;
            foreach (var page in vInfo.PagesInfo)
            {
                if (more && page.index != vInfo.PagesInfo.Count)
                {
                    continue;
                }
                
                if (!more && page.index > 5)
                {
                    Logger.LogInformation("......");
                    more = true;
                    continue;
                }
                
                Logger.LogInformation("P{Index}: Cid={Cid} Title=\"{Title}\" Duration={FormatTime}",
                    page.index, page.cid, page.title, Utils.FormatTime(page.dur));
            }
        }
        
        ViewModel.ApiType = ViewModel.Mode == DownloadMode.Tv ? "TV" : (ViewModel.UseIntlApi ? "INTL" : "WEB");
        ViewModel.Pages.AddRange(vInfo.PagesInfo
            .Select(page => new VideoPageInfo
            {
                Index = page.index,
                Aid = page.aid,
                Title = page.title,
                DurationInt = page.dur
            }));
    }
}