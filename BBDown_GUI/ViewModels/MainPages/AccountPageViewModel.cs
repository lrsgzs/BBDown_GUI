using System.Threading.Tasks;
using BBDown_GUI.Models;
using BBDown_GUI.Services;
using BBDown_GUI.Services.Config;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BBDown_GUI.ViewModels.MainPages;

public partial class AccountPageViewModel : ObservableRecipient
{
    public ConfigModel Config { get; }
    public BiliBiliLoginService BiliBiliLoginService { get; }
    
    [ObservableProperty] private string _qrcodeUrl = string.Empty;
    
    // web 登录
    [ObservableProperty] private bool _isWebLogged = false;
    [ObservableProperty] private bool _isWebLoginBusy = false;
    [ObservableProperty] private WebLoginData _webLoginData = new();
    [ObservableProperty] private LoginStatus _webLoginStatus = new();
    
    // tv 登录
    [ObservableProperty] private bool _isTvLogged = false;
    [ObservableProperty] private bool _isTvLoginBusy = false;
    [ObservableProperty] private TvLoginData _tvLoginData = new();
    [ObservableProperty] private LoginStatus _tvLoginStatus = new();

    public AccountPageViewModel(ConfigHandler configHandler, BiliBiliLoginService biliBiliLoginService)
    {
        Config = configHandler.Data;
        BiliBiliLoginService = biliBiliLoginService;
    }

    public async Task Initialize()
    {
        IsWebLogged = await BiliBiliLoginService.CheckWebLogin();
        IsTvLogged = await BiliBiliLoginService.CheckTvLogin();
    }
}