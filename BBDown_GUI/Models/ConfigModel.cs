using CommunityToolkit.Mvvm.ComponentModel;

namespace BBDown_GUI.Models;

public partial class ConfigModel : ObservableRecipient
{
    [ObservableProperty] private string _test = "hello";
    
    [ObservableProperty] private string _webCookie = string.Empty;
    [ObservableProperty] private string _tvToken = string.Empty;
}