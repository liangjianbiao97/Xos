using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Media;

namespace Xos.Component;

internal class GridLine : Control
{
    private bool _isLayout;
    private XDesk _desk;
    // private readonly NumericUpDown _scalebox;

    public GridLine(XDesk desk)
    {
        // _scalebox = new NumericUpDown
        // {
        //     FormatString = "N0", Value = 100, Minimum = 50, Maximum = 200, Increment = 10
        // };
        //
        // _scalebox.ValueChanged += (sender, args) =>
        // {
        //     desk.RenderTransform = new ScaleTransform()
        //     {
        //         ScaleX = (double)_scalebox.Value / 100,
        //         ScaleY = (double)_scalebox.Value / 100
        //     };
        //     desk.Width = desk.Bounds.Width / ((double)_scalebox.Value / 100);
        //     desk.Height = desk.Bounds.Height / ((double)_scalebox.Value / 100);
        // };
        
        _desk = desk;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var cellSize = 10;
        var curSize = cellSize * 10;
        var bounds = new Rect(0, 0, _desk.Bounds.Width, _desk.Bounds.Height);
        var thinpen = new Pen(Brushes.Gray, 0.3);
        var boldpen = new Pen(Brushes.Gray, 0.5);
        // 垂直线
        for (double x = 0; x < bounds.Width; x += cellSize)
        {
            context.DrawLine(x % curSize == 0 ? boldpen : thinpen, new Point(x, 0), new Point(x, bounds.Height));
        }

        // 水平线
        for (double y = 0; y < bounds.Height; y += cellSize)
        {
            context.DrawLine(y % curSize == 0 ? boldpen : thinpen, new Point(0, y), new Point(bounds.Width, y));
        }
    }

    internal void Show()
    {
        _desk.Children.Add(this);
        // _desk.Children.Add(_scalebox);
        _isLayout = true;
    }

    internal void Hide()
    {
        _desk.Children.Remove(this);
        // _desk.Children.Remove(_scalebox);
        _isLayout = false;
    }

    internal void Update()
    {
        if (_isLayout) InvalidateVisual();
    }
}