using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BBDown_GUI.Attributes;

namespace BBDown_GUI.Views.MainPages;

[MainPageInfo("设置", "settings", "\uEF27")]
public partial class SettingsPage : UserControl
{
    public SettingsPage()
    {
        InitializeComponent();
    }
}