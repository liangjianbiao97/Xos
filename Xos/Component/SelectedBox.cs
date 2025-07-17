using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Xos.Component;

internal enum TransfromHandle
{
    None,
    TopLeft,
    TopMiddle,
    TopRight,
    MiddleLeft,
    MiddleRight,
    BottomLeft,
    BottomMiddle,
    BottomRight,
    Move
}

internal class SelectedBox(XDesk desk) : Control
{
    private Rect _viewBounds;
    private bool _isSelected;
    private const double HandleSize = 8;
    private const double HandleHitTestSize = 12;


    public override void Render(DrawingContext context)
    {
        base.Render(context);

        // 绘制选择框
        if (!_isSelected) return;
        DrawResizeHandles(context, _viewBounds);
    }


    public TransfromHandle Handle { get; private set; }

    internal void Show(Rect viewBounds)
    {
        _viewBounds = viewBounds;
        if (!_isSelected) desk.Children.Add(this);
        _isSelected = true;
        InvalidateVisual(); // 触发重绘
    }

    internal void Hide()
    {
        if (!_isSelected) return;
        _isSelected = false;
        InvalidateVisual();
        desk.Children.Remove(this);
    }


    // 获取指定点的操作点
    internal void GetHandleAtPoint(Point point)
    {
        Handle = _viewBounds.Contains(point) ? TransfromHandle.Move : TransfromHandle.None;
        // 检查各个操作点区域
        if (IsPointInHandle(point, _viewBounds.TopLeft))
            Handle = TransfromHandle.TopLeft;
        if (IsPointInHandle(point, new Point(_viewBounds.X + _viewBounds.Width / 2, _viewBounds.Y)))
            Handle = TransfromHandle.TopMiddle;
        if (IsPointInHandle(point, _viewBounds.TopRight))
            Handle = TransfromHandle.TopRight;
        if (IsPointInHandle(point, new Point(_viewBounds.X, _viewBounds.Y + _viewBounds.Height / 2)))
            Handle = TransfromHandle.MiddleLeft;
        if (IsPointInHandle(point,
                new Point(_viewBounds.X + _viewBounds.Width, _viewBounds.Y + _viewBounds.Height / 2)))
            Handle = TransfromHandle.MiddleRight;
        if (IsPointInHandle(point, _viewBounds.BottomLeft))
            Handle = TransfromHandle.BottomLeft;
        if (IsPointInHandle(point,
                new Point(_viewBounds.X + _viewBounds.Width / 2, _viewBounds.Y + _viewBounds.Height)))
            Handle = TransfromHandle.BottomMiddle;
        if (IsPointInHandle(point, _viewBounds.BottomRight))
            Handle = TransfromHandle.BottomRight;

        if (!_isSelected) Handle = TransfromHandle.None;
    }

    // 检查点是否在操作点区域内
    private static bool IsPointInHandle(Point point, Point handleCenter)
    {
        return point.X >= handleCenter.X - HandleHitTestSize / 2 &&
               point.X <= handleCenter.X + HandleHitTestSize / 2 &&
               point.Y >= handleCenter.Y - HandleHitTestSize / 2 &&
               point.Y <= handleCenter.Y + HandleHitTestSize / 2;
    }

    // 绘制操作点
    private static void DrawResizeHandles(DrawingContext context, Rect rect)
    {
        // 计算所有操作点位置
        var handles = new Dictionary<TransfromHandle, Point>
        {
            [TransfromHandle.TopLeft] = rect.TopLeft,
            [TransfromHandle.TopMiddle] = new Point(rect.X + rect.Width / 2, rect.Y),
            [TransfromHandle.TopRight] = rect.TopRight,
            [TransfromHandle.MiddleLeft] = new Point(rect.X, rect.Y + rect.Height / 2),
            [TransfromHandle.MiddleRight] = new Point(rect.X + rect.Width, rect.Y + rect.Height / 2),
            [TransfromHandle.BottomLeft] = rect.BottomLeft,
            [TransfromHandle.BottomMiddle] = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height),
            [TransfromHandle.BottomRight] = rect.BottomRight
        };

        // 绘制每个操作点
        foreach (var handle in handles)
        {
            var handleRect = new Rect(
                handle.Value.X - HandleSize / 2,
                handle.Value.Y - HandleSize / 2,
                HandleSize,
                HandleSize
            );

            // 活动操作点高亮
            var brush = Brushes.Gold;
            var pen = new Pen(Brushes.DarkOrange, 0.5);

            context.DrawRectangle(brush, pen, handleRect);
        }
    }
}