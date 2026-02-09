using Avalonia.Controls;
using Avalonia.Interactivity;
using BBDown_GUI.Attributes;
using BBDown_GUI.Helpers.UI;

namespace BBDown_GUI.Views.MainPages;

[MainPageInfo("调试", "debug", "\uE2C8")]
public partial class DebugPage : UserControl
{
    public DebugPage()
    {
        InitializeComponent();
    }

    private void ShowToastButton_OnClick(object? sender, RoutedEventArgs e)
    {
        this.ShowToast("测试测试~");
    }
}