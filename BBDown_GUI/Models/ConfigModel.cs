using CommunityToolkit.Mvvm.ComponentModel;

namespace BBDown_GUI.Models;

public partial class ConfigModel : ObservableRecipient
{
    [ObservableProperty] private string _test = "hello";
}