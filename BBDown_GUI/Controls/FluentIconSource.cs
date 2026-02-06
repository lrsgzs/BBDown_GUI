using Avalonia.Media;
using FluentAvalonia.UI.Controls;

namespace BBDown_GUI.Controls;

/// <summary>
/// Fluent Icon 图标源
/// </summary>
public class FluentIconSource : FontIconSource
{
    public FluentIconSource()
    {
        FontFamily = new FontFamily("avares://BBDown_GUI/Assets/Fonts/#FluentSystemIcons-Resizable");
    }
    
    public FluentIconSource(string glyph) : this()
    {
        Glyph = glyph;
    }

    public FluentIconSource ProvideValue() => this;
}