using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using BBDown_GUI.Abstraction;
using BBDown_GUI.Attributes;
using BBDown_GUI.Models;
using BBDown_GUI.Services;
using BBDown_GUI.ViewModels.MainPages;
using Microsoft.Extensions.Logging;

namespace BBDown_GUI.Views.MainPages;

[MainPageInfo("用户", "user", "\uECE5")]
public partial class UserPage : UserControl
{
    public UserPageViewModel ViewModel { get; } = IAppHost.GetService<UserPageViewModel>();

    private ILogger<UserPage> Logger { get; } = IAppHost.GetService<ILogger<UserPage>>();
    private BiliBiliLoginService BiliBiliLoginService { get; } = IAppHost.GetService<BiliBiliLoginService>();
    
    public UserPage()
    {
        DataContext = this;
        InitializeComponent();

        _ = ViewModel.Initialize();
    }

    private async void RefreshLoginStatusButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            Logger.LogInformation("刷新登录状态...");
            ViewModel.IsWebLogged = await BiliBiliLoginService.CheckWebLogin();
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "刷新登录状态中出现错误。");
        }
    }

    private async void RefreshQrCodeButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            Logger.LogInformation("开始登录...");
            ViewModel.IsWebLoginBusy = true;
            
            var data = await BiliBiliLoginService.GenerateWebLoginData();
            ViewModel.WebLoginData = data;
            
            await foreach (var status in BiliBiliLoginService.WaitLoginWeb(data))
            {
                ViewModel.WebLoginStatus = status;
            }
            
            // 重置
            ViewModel.WebLoginData = new LoginData();
            ViewModel.IsWebLogged = await BiliBiliLoginService.CheckWebLogin();
            ViewModel.IsWebLoginBusy = false;
            
            Logger.LogInformation("登录已结束。");
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "登录中出现错误。");
            
            // 重置
            ViewModel.WebLoginData = new LoginData();
            ViewModel.WebLoginStatus = new LoginStatus(false, -1, "登录过程中出现错误。");
            ViewModel.IsWebLoginBusy = false;
        }
    }
}