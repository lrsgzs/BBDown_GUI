using System.Collections.ObjectModel;
using BBDown_GUI.Enums;
using BBDown_GUI.Models;
using BBDown_GUI.Models.Video;
using BBDown_GUI.Services;
using BBDown_GUI.Services.Config;
using BBDown.Core.Entity;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BBDown_GUI.ViewModels.MainPages;

public partial class DownloadPageViewModel : ObservableRecipient
{
    public ConfigModel Config { get; }
    public BiliBiliLoginService BiliBiliLoginService { get; }
    
    [ObservableProperty] private int _pageIndex = 0;
    
    // step 1
    [ObservableProperty] private string _videoLink = string.Empty;
    [ObservableProperty] private DownloadMode _mode = DownloadMode.Web;
    [ObservableProperty] private bool _useIntlApi = false;
    
    // step 1 result
    [ObservableProperty] private VInfo? _videoInfo = null;
    [ObservableProperty] private string _aidOri = string.Empty;
    [ObservableProperty] private string _apiType = string.Empty;
    [ObservableProperty] private ObservableCollection<VideoPageInfo> _pages = [];
    
    // step 2
    [ObservableProperty] private int _selectedPage = 1;
    
    public DownloadPageViewModel(ConfigHandler configHandler, BiliBiliLoginService biliBiliLoginService)
    {
        Config = configHandler.Data;
        BiliBiliLoginService = biliBiliLoginService;
    }
}