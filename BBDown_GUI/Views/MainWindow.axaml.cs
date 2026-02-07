using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using BBDown_GUI.Controls;
using BBDown_GUI.Helpers;
using FluentAvalonia.UI.Windowing;

namespace BBDown_GUI.Views;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        SplashScreen = new EmptySplashScreen();
        InitializeComponent();

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
    }
    
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        var isMicaSupported = OperatingSystem.IsWindows() 
                              && Environment.OSVersion.Version >= new Version(10, 0, 22000, 0)
                              && AvaloniaUnsafeAccessorHelpers.GetActiveWin32CompositionMode() == AvaloniaUnsafeAccessorHelpers.Win32CompositionMode.WinUIComposition;
        if (isMicaSupported)
        {
            TransparencyLevelHint = [WindowTransparencyLevel.Mica];
            Background = Brushes.Transparent;
        }
    }
}