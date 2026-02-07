using System.Collections.ObjectModel;
using BBDown_GUI.Attributes;
using BBDown_GUI.Models;
using BBDown_GUI.Services.Config;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;

namespace BBDown_GUI.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    public ConfigModel Config { get; }
    
    [ObservableProperty] private object? _frameContent;
    [ObservableProperty] private MainPageInfo? _selectedPageInfo = null;
    [ObservableProperty] private NavigationViewItemBase? _selectedNavigationViewItem = null;
    public ObservableCollection<NavigationViewItemBase> NavigationViewItems { get; } = [];
    public ObservableCollection<NavigationViewItemBase> NavigationViewFooterItems { get; } = [];

    public MainViewModel(ConfigHandler handler)
    {
        Config = handler.Data;
    }
}