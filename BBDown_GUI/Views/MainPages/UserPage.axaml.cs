using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using BBDown_GUI.Abstraction;
using BBDown_GUI.Attributes;
using BBDown_GUI.Models;
using BBDown_GUI.Services;
using BBDown_GUI.Services.Config;
using BBDown_GUI.ViewModels.MainPages;
using Microsoft.Extensions.Logging;

namespace BBDown_GUI.Views.MainPages;

[MainPageInfo("用户", "user", "\uECE5")]
public partial class UserPage : UserControl
{
    public UserPageViewModel ViewModel { get; } = IAppHost.GetService<UserPageViewModel>();
    public BiliBiliLoginService BiliBiliLoginService { get; } = IAppHost.GetService<BiliBiliLoginService>();
    public ConfigHandler ConfigHandler { get; } = IAppHost.GetService<ConfigHandler>();

    private ILogger<UserPage> Logger { get; } = IAppHost.GetService<ILogger<UserPage>>();
    
    public UserPage()
    {
        DataContext = this;
        InitializeComponent();

        _ = ViewModel.Initialize();
    }

    private async void WebRefreshLoginStatusButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            Logger.LogInformation("刷新 Web 版登录状态...");
            ViewModel.IsWebLogged = await BiliBiliLoginService.CheckWebLogin();
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "刷新 Web 版登录状态中出现错误。");
        }
    }

    private async void WebRefreshQrCodeButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            Logger.LogInformation("开始登录 Web 版...");
            ViewModel.IsWebLoginBusy = true;
            
            var data = await BiliBiliLoginService.GenerateWebLoginData();
            ViewModel.WebLoginData = data;
            
            await foreach (var status in BiliBiliLoginService.WaitLoginWeb(data))
            {
                ViewModel.WebLoginStatus = status;
            }
            
            // 重置
            ViewModel.WebLoginData = new WebLoginData();
            ViewModel.IsWebLogged = await BiliBiliLoginService.CheckWebLogin();
            ViewModel.IsWebLoginBusy = false;
            
            Logger.LogInformation("Web 版登录已结束。");
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Web 版登录中出现错误。");
            
            // 重置
            ViewModel.WebLoginData = new WebLoginData();
            ViewModel.WebLoginStatus = new LoginStatus(false, -1, "Web 版登录过程中出现错误。");
            ViewModel.IsWebLoginBusy = false;
        }
    }

    private async void WebLogoutButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            ConfigHandler.Data.WebCookie = string.Empty;
            await BiliBiliLoginService.CheckWebLogin();
            ViewModel.IsWebLogged = false;
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Web 版登出中出现错误。");
        }
    }

    private async void TvRefreshLoginStatusButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            Logger.LogInformation("刷新 TV 版登录状态...");
            ViewModel.IsTvLogged = await BiliBiliLoginService.CheckTvLogin();
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "刷新 TV 版登录状态中出现错误。");
        }
    }

    private async void TvRefreshQrCodeButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            Logger.LogInformation("开始登录 TV 版...");
            ViewModel.IsTvLoginBusy = true;
            
            var data = await BiliBiliLoginService.GenerateTvLoginData();
            ViewModel.TvLoginData = data;
            
            await foreach (var status in BiliBiliLoginService.WaitLoginTv(data))
            {
                ViewModel.TvLoginStatus = status;
            }
            
            // 重置
            ViewModel.TvLoginData = new TvLoginData();
            ViewModel.IsTvLogged = await BiliBiliLoginService.CheckTvLogin();
            ViewModel.IsTvLoginBusy = false;
            
            Logger.LogInformation("TV 版登录已结束。");
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "TV 版登录中出现错误。");
            
            // 重置
            ViewModel.TvLoginData = new TvLoginData();
            ViewModel.TvLoginStatus = new LoginStatus(false, -1, "TV 版登录过程中出现错误。");
            ViewModel.IsTvLoginBusy = false;
        }
    }

    private async void TvLogoutButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            ConfigHandler.Data.TvToken = string.Empty;
            await BiliBiliLoginService.CheckTvLogin();
            ViewModel.IsTvLogged = false;
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "TV 版登出中出现错误。");
        }
    }
}