using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using BBDown_GUI.Controls;
using FluentAvalonia.UI.Windowing;

namespace BBDown_GUI.Views;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        SplashScreen = new EmptySplashScreen();
        InitializeComponent();
    }
    
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        TransparencyLevelHint = [WindowTransparencyLevel.Mica];
        Background = Brushes.Transparent;
    }
}