using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BBDown_GUI.Attributes;

namespace BBDown_GUI.Views.MainPages;

[MainPageInfo("下载视频", "download", "\uE0D3")]
public partial class DownloadPage : UserControl
{
    public DownloadPage()
    {
        InitializeComponent();
    }
}