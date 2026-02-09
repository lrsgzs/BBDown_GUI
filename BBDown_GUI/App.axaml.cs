using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using BBDown_GUI.Abstraction;
using BBDown_GUI.Extensions.Registry;
using BBDown_GUI.Services;
using BBDown_GUI.Services.Config;
using BBDown_GUI.Services.Logging;
using BBDown_GUI.ViewModels;
using BBDown_GUI.ViewModels.MainPages;
using BBDown_GUI.Views;
using BBDown_GUI.Views.MainPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace BBDown_GUI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        BuildHost();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                Content = IAppHost.GetService<MainView>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = IAppHost.GetService<MainView>();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private void BuildHost()
    {
        IAppHost.Host = Host
            .CreateDefaultBuilder()
            .UseContentRoot(AppContext.BaseDirectory)
            .ConfigureServices(services =>
            {
                // 日志
                services.AddLogging(builder =>
                {
                    builder.AddConsoleFormatter<ClassIslandConsoleFormatter, ConsoleFormatterOptions>();
                    builder.AddConsole(console => { console.FormatterName = "classisland"; });
#if DEBUG
                    builder.SetMinimumLevel(LogLevel.Trace);
#endif
                });
                
                // 配置
                services.AddSingleton<ConfigService>();
                services.AddSingleton<ConfigHandler>();
                
                // 服务
                services.AddSingleton<BiliBiliLoginService>();
                
                // 主窗口
                services.AddSingleton<MainView>();
                services.AddTransient<MainViewModel>();
                
                // 界面 Views
                services.AddMainPage<HomePage>();
                services.AddMainPageSeparator();
                services.AddMainPage<AccountPage>();
                services.AddMainPage<DownloadPage>();

                services.AddMainPageFooter<SettingsPage>();
                services.AddMainPageFooter<AboutPage>();
                services.AddMainPageFooterSeparator();
                services.AddMainPageFooter<DebugPage>();
                
                // 界面 ViewModels
                services.AddTransient<DownloadPageViewModel>();
                services.AddTransient<AccountPageViewModel>();
            })
            .Build();

        var logger = IAppHost.GetService<ILogger<App>>();
        logger.LogInformation("BBDown_GUI Copyright by lrs2187(2026) Licensed under GPL3.0");
        logger.LogInformation("Host built.");
        IAppHost.GetService<ConfigHandler>();
    }
}