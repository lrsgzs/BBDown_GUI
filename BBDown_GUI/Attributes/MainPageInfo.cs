using System;

namespace BBDown_GUI.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class MainPageInfo(string name, string id, string iconGlyph = "\uE06F", bool useFullWidth = false, bool hidePageTitle = false) : Attribute
{
    public string Name { get; } = name;
    public string Id { get; } = id;
    public string IconGlyph { get; } = iconGlyph;
    public bool UseFullWidth { get; } = useFullWidth;
    public bool HidePageTitle { get; } = hidePageTitle;
}