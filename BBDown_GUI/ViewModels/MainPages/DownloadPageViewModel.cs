using BBDown_GUI.Models;
using BBDown_GUI.Services;
using BBDown_GUI.Services.Config;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BBDown_GUI.ViewModels.MainPages;

public partial class DownloadPageViewModel : ObservableRecipient
{
    public ConfigModel Config { get; }
    public BiliBiliLoginService BiliBiliLoginService { get; }
    
    public DownloadPageViewModel(ConfigHandler configHandler, BiliBiliLoginService biliBiliLoginService)
    {
        Config = configHandler.Data;
        BiliBiliLoginService = biliBiliLoginService;
    }
}