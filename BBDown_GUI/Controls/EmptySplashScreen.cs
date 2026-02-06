using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using FluentAvalonia.UI.Windowing;

namespace BBDown_GUI.Controls;

public class EmptySplashScreen : IApplicationSplashScreen
{
    public async Task RunTasks(CancellationToken cancellationToken) { }

    public string AppName { get; } = "BBDown_GUI";
    public IImage AppIcon { get; } =
        new Bitmap(AssetLoader.Open(new Uri("avares://BBDown_GUI/Assets/AppLogo.png")));
    public object? SplashScreenContent { get; } = null;
    public int MinimumShowTime { get; } = 1000;
}