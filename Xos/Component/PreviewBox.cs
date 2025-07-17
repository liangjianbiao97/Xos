using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Xos.Component;

internal class PreviewBox(XDesk desk) : Control
{
    private Rect _initialBounds;
    private Rect _previewBounds;
    private Point _dragStartPoint;
    public bool IsPreviewing;

    public double DeltaX => _previewBounds.X - _initialBounds.X;
    public double DeltaY => _previewBounds.Y - _initialBounds.Y;
    public double DeltaWidth => _previewBounds.Width - _initialBounds.Width;
    public double DeltaHeight => _previewBounds.Height - _initialBounds.Height;

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        // 绘制选择框
        if (!IsPreviewing) return;

        var brush = new SolidColorBrush(0x206495ED);
        var pen = new Pen(Brushes.DeepSkyBlue, 0.5);

        context.DrawRectangle(brush, pen, _previewBounds);
    }


    public TransfromHandle Handle { get; private set; }

    internal void Show(Rect viewBounds, Point dragStart, TransfromHandle handle)
    {
        _initialBounds = viewBounds;
        _previewBounds = viewBounds;
        _dragStartPoint = dragStart;
        Handle = handle;
        if (!IsPreviewing) desk.Children.Add(this);
        IsPreviewing = true;
    }

    public void Update(Point current)
    {
        var vector = current - _dragStartPoint;
        double left = _initialBounds.X;
        double top = _initialBounds.Y;
        double width = _initialBounds.Width;
        double height = _initialBounds.Height;
        switch (Handle)
        {
            case TransfromHandle.TopLeft: // 左上
                width = Math.Max(6, width - vector.X);
                height = Math.Max(6, height - vector.Y);
                left = left + _initialBounds.Width - width;
                top = top + _initialBounds.Height - height;
                break;
            case TransfromHandle.TopMiddle: // 上中
                height = Math.Max(6, height - vector.Y);
                top = top + _initialBounds.Height - height;
                break;
            case TransfromHandle.TopRight: // 右上
                width = Math.Max(6, width + vector.X);
                height = Math.Max(6, height - vector.Y);
                top = top + _initialBounds.Height - height;
                break;
            case TransfromHandle.MiddleRight: // 右中
                width += vector.X;
                break;
            case TransfromHandle.BottomRight: // 右下
                width += vector.X;
                height += vector.Y;
                break;
            case TransfromHandle.BottomMiddle: // 下中
                height += vector.Y;
                break;
            case TransfromHandle.BottomLeft: // 左下
                width = Math.Max(6, width - vector.X);
                height = Math.Max(6, height + vector.Y);
                left = left + _initialBounds.Width - width;
                break;
            case TransfromHandle.MiddleLeft: // 左中
                width = Math.Max(6, width - vector.X);
                left = left + _initialBounds.Width - width;
                break;
            case TransfromHandle.Move: // 移动
                left += vector.X;
                top += vector.Y;
                break;
        }

        // 应用最小尺寸限制
        width = Math.Max(6, width);
        height = Math.Max(6, height);

        _previewBounds = new Rect(left, top, width, height);
        InvalidateVisual();
    }

    internal void Hide()
    {
        if (!IsPreviewing) return;
        IsPreviewing = false;
        InvalidateVisual();
        desk.Children.Remove(this);
    }
}