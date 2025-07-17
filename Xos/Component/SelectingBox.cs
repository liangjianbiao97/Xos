using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Xos.Component;

public class SelectingBox(XDesk desk) : Control
{
    private Point? _selectionStart;
    private Point? _currentPoint;
    private Rect _bounds;
    public bool IsSelecting;
    private bool _direction;

    internal void Show(Point position)
    {
        desk.Children.Add(this);
        _selectionStart = position;
        _currentPoint = _selectionStart;
        IsSelecting = true;
        InvalidateVisual(); // 触发重绘
    }

    internal void Update(Point position)
    {
        if (!IsSelecting) return;
        _currentPoint = position;
        InvalidateVisual();
    }

    internal void Hide()
    {
        if (!IsSelecting) return;
        IsSelecting = false;
        // 获取最终选择区域
        if (_selectionStart.HasValue && _currentPoint.HasValue)
        {
            _bounds = CalculateSelectionRect(_selectionStart.Value, _currentPoint.Value);
        }

        // 清除选择框
        _selectionStart = null;
        _currentPoint = null;
        InvalidateVisual();
        desk.Children.Remove(this);
    }

    internal IEnumerable<XView> MacthViews(IEnumerable<XView> xViews)
    {
        foreach (var view in xViews)
        {
            if (_bounds is { Width: < 5, Height: < 5 }) break;
            if (_direction && _bounds.Contains(view.GetBounds())) yield return view;
            if (!_direction && _bounds.Intersects(view.GetBounds())) yield return view;
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        // 绘制选择框
        if (!IsSelecting || !_selectionStart.HasValue || !_currentPoint.HasValue) return;
        var rect = CalculateSelectionRect(_selectionStart.Value, _currentPoint.Value);
        _direction = _selectionStart.Value.X > _currentPoint.Value.X;
        if (_direction)
        {
            var brush = new SolidColorBrush(0x206495ED);
            var pen = new Pen(Brushes.DeepSkyBlue, 0.3);
            context.DrawRectangle(brush, pen, rect);
        }
        else
        {
            var brush = new SolidColorBrush(0x2064ED95);
            var pen = new Pen(Brushes.SpringGreen, 0.3);
            context.DrawRectangle(brush, pen, rect);
        }
    }

    // 确保矩形总是左上角到右下角格式
    private static Rect CalculateSelectionRect(Point start, Point end)
    {
        return new Rect(
            Math.Min(start.X, end.X),
            Math.Min(start.Y, end.Y),
            Math.Abs(end.X - start.X),
            Math.Abs(end.Y - start.Y)
        );
    }
}