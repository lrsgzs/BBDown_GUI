using BBDown_GUI.Models;
using BBDown_GUI.Services.Config;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BBDown_GUI.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    public ConfigModel Config { get; }

    public MainViewModel(ConfigHandler handler)
    {
        Config = handler.Data;
    }
}