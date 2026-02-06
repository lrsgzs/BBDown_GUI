using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using BBDown_GUI.Abstraction;
using BBDown_GUI.Services.Config;
using BBDown_GUI.ViewModels;

namespace BBDown_GUI.Views;

public partial class MainView : UserControl
{
    public MainViewModel ViewModel { get; } = IAppHost.GetService<MainViewModel>();
    
    public MainView()
    {
        DataContext = this;
        InitializeComponent();
        
        RenderOptions.SetTextRenderingMode(this, TextRenderingMode.Antialias);
        RenderOptions.SetBitmapInterpolationMode(this, BitmapInterpolationMode.HighQuality);
        RenderOptions.SetEdgeMode(this, EdgeMode.Antialias);
    }
    
    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        DataContext = null;
        IAppHost.GetService<ConfigHandler>().Save();
    }
}