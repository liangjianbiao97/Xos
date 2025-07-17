using System;
using Avalonia.Controls;

namespace Xos.Module;

public enum ModuleGroup
{
    Shape,
    Chart,
    System,
    Control,
    Status
}

public class ViewInfo
{
    public ModuleGroup Group { get; set; }
    public string? ViewName { get; set; }
    public double InitWidth { get; set; }
    public double InitHeight { get; set; }
    public Func<UserControl>? GetView { get; set; }
}