using Avalonia.Controls;
using BBDown_GUI.Abstraction;
using BBDown_GUI.Attributes;
using BBDown_GUI.Services;
using BBDown_GUI.Services.Config;
using BBDown_GUI.ViewModels.MainPages;

namespace BBDown_GUI.Views.MainPages;

[MainPageInfo("下载视频", "download", "\uE0D3")]
public partial class DownloadPage : UserControl
{
    public DownloadPageViewModel ViewModel { get; } = IAppHost.GetService<DownloadPageViewModel>();
    public ConfigHandler ConfigHandler { get; } = IAppHost.GetService<ConfigHandler>();
    public BiliBiliLoginService BiliBiliLoginService { get; } = IAppHost.GetService<BiliBiliLoginService>();
    
    public DownloadPage()
    {
        InitializeComponent();
    }
}