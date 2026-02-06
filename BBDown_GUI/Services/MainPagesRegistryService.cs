using System.Collections.ObjectModel;
using BBDown_GUI.Attributes;

namespace BBDown_GUI.Services;

public static class MainPagesRegistryService
{
    public static ObservableCollection<MainPageInfo> Items { get; } = [];
    public static ObservableCollection<MainPageInfo> FooterItems { get; } = [];
}