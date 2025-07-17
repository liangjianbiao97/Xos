using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Xos.Component;
using Xos.Operation;

namespace Xos;

public class XDesk : Canvas
{
    private readonly XPackage package;
    
    private readonly SuggestBox _suggsetBox;
    private readonly GridLine _gridLine;
    private readonly SelectingBox _selectingBox;
    private readonly SelectedBox _selectedBox;
    private readonly PreviewBox _previewBox;
    private readonly AlignmentToolbar _toolbar;

    private readonly DispatcherTimer _updateTimer;
    private Point _currentPosition;
    private Point _startPosition;
    private bool _canEdit;
    private readonly ScaleTransform _scale = new() { ScaleX = 1, ScaleY = 1 };
    private readonly TranslateTransform _translate = new() { X = 0, Y = 0 };

    internal List<XView> SelectedElements { get; } = [];

    internal Rect SelectingBounds
    {
        get
        {
            if (SelectedElements.Count == 0) return default;

            var minX = double.MaxValue;
            var minY = double.MaxValue;
            var maxX = double.MinValue;
            var maxY = double.MinValue;

            foreach (var element in SelectedElements.OfType<XView>())
            {
                if (!Children.Contains(element)) continue;

                var left = GetLeft(element);
                var top = GetTop(element);
                var right = left + element.Width;
                var bottom = top + element.Height;

                minX = Math.Min(minX, left);
                minY = Math.Min(minY, top);
                maxX = Math.Max(maxX, right);
                maxY = Math.Max(maxY, bottom);
            }

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }
    }

    internal Rect UsingBounds
    {
        get
        {
            if (Children.Count == 0) return default;

            var minX = double.MaxValue;
            var minY = double.MaxValue;
            var maxX = double.MinValue;
            var maxY = double.MinValue;

            foreach (var element in Children.OfType<XView>())
            {
                var left = GetLeft(element);
                var top = GetTop(element);
                var right = left + element.Width;
                var bottom = top + element.Height;

                minX = Math.Min(minX, left);
                minY = Math.Min(minY, top);
                maxX = Math.Max(maxX, right);
                maxY = Math.Max(maxY, bottom);
            }

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }
    }

    public XDesk()
    {
        // 基础设置
        Width = 1600;
        Height = 900;
        Background = Brushes.Transparent;
        ClipToBounds = true;
        Focusable = true;
        RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);
        RenderTransform = _scale;
        
        // 创建变换组合
        var transformGroup = new TransformGroup();
        transformGroup.Children.Add(_scale);
        transformGroup.Children.Add(_translate);
        RenderTransform = transformGroup;

        // 组件初始化
        _suggsetBox = new(this);
        _gridLine = new(this);
        _selectingBox = new(this);
        _selectedBox = new(this);
        _previewBox = new(this);
        _toolbar = new(this);

        // 事件绑定
        PointerPressed += OnMouseLeftButtonDown;
        PointerMoved += OnMouseMove;
        PointerReleased += OnMouseLeftButtonUp;
        KeyDown += OnKeyDown;
        PointerWheelChanged += OnWheelChanged;
        
        //package
        package = new XPackage(this,"XViews");
        var a = new XPackage(this,"XViews");
        

        // 初始化更新计时器
        _updateTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(50),
            IsEnabled = false
        };
        _updateTimer.Tick += (_, _) => UpdateSelectedComponents();
    }


    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        _gridLine.Update();
    }

    private void OnWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (!_canEdit) return;
        var inc = e.KeyModifiers == KeyModifiers.Control ? 0.1 : 0.01;
        if (e.Delta.Y > 0)
        {
            _scale.ScaleX += inc;
            _scale.ScaleY += inc;
        }
        else
        {
            _scale.ScaleX -= inc;
            _scale.ScaleY -= inc;
        }
    }

    private void OnMouseLeftButtonDown(object? sender, PointerPressedEventArgs e)
    {
        _startPosition = e.GetPosition(this);
        Focus();
        if (!_canEdit) return;
        if (e.Properties.IsLeftButtonPressed)
        {
            if (_selectedBox.Handle == TransfromHandle.None) _selectingBox.Show(e.GetPosition(this));
            else _previewBox.Show(SelectingBounds, e.GetPosition(this), _selectedBox.Handle);
            e.Pointer.Capture(this);
            e.Handled = true;
        }
    }

    private void OnMouseMove(object? sender, PointerEventArgs e)
    {
        _currentPosition = e.GetPosition(this);
        _selectedBox.GetHandleAtPoint(_currentPosition);
        // 在标题栏显示坐标（实际使用时可以替换为状态栏更新）
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime
            {
                MainWindow: not null
            } window)
            window.MainWindow.Title =
                $"网格坐标: [{_currentPosition.X:F1}, {_currentPosition.Y:F1}] {_selectedBox.Handle.ToString()}";
        if (!_canEdit) return;

        if (e.Properties.IsMiddleButtonPressed)
        {
            _translate.X += _currentPosition.X - _startPosition.X;
            _translate.Y += _currentPosition.Y - _startPosition.Y;
            Cursor = new Cursor(StandardCursorType.DragMove);
        }
        else
        {
            Cursor = Cursor.Default;
        }

        if (_previewBox.IsPreviewing)
            _previewBox.Update(_currentPosition);
        else if (_selectingBox.IsSelecting)
            _selectingBox.Update(_currentPosition);
    }

    private void OnMouseLeftButtonUp(object? sender, PointerReleasedEventArgs e)
    {
        if (!_canEdit) return;
        HitSelection(e.GetPosition(this));
        if (_selectingBox.IsSelecting)
        {
            _selectingBox.Hide();
            BoxSelection();
        }

        //拖到控制点
        if (_previewBox.IsPreviewing)
        {
            // 应用变换到实际元素
            ApplyTransformation();
            // 清除预览
            _previewBox.Hide();
        }

        e.Pointer.Capture(null);
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e is { KeyModifiers: KeyModifiers.Control, Key: Key.E })
        {
            _canEdit = !_canEdit;
            InvalidateVisual();
            ClrSelection();
            if (_canEdit)
                _gridLine.Show();
            else
                _gridLine.Hide();
        }

        if (!_canEdit) return;

        if (e.KeyModifiers == KeyModifiers.None)
        {
            switch (e)
            {
                case { Key: Key.Enter }:
                    if (!_suggsetBox.IsActivated) _suggsetBox.Show(_currentPosition);
                    e.Handled = true;
                    break;
                case { Key: Key.Escape }:
                    ClrSelection();
                    if (_suggsetBox.IsActivated) _suggsetBox.Hide();
                    e.Handled = true;
                    break;
                case { Key: Key.Delete }:
                    DeleteSelectedElements();
                    break;
            }
        }

        if (e.KeyModifiers == KeyModifiers.Control)
        {
            switch (e)
            {
                case { Key: Key.Z }:
                    Operator.Undo();
                    break;
                case { Key: Key.Y }:
                    Operator.Redo();
                    break;
                case { Key: Key.A }:
                    AllSelection();
                    break;
            }
        }
    }

    private void HitSelection(Point point)
    {
        foreach (var element in Children.OfType<XView>().OrderByDescending(x => x.Z))
        {
            var elementBounds = new Rect(
                GetLeft(element),
                GetTop(element),
                element.Width,
                element.Height);
            if (!elementBounds.Contains(point)) continue;
            if (SelectedElements.Contains(element)) continue;
            SelectedElements.Add(element);
            return;
        }
    }

    private void BoxSelection()
    {
        foreach (XView element in _selectingBox.MacthViews(Children.OfType<XView>()))
        {
            if (!SelectedElements.Contains(element)) SelectedElements.Add(element);
        }

        RequestUpdate();
    }

    private void AllSelection()
    {
        foreach (XView element in Children.OfType<XView>())
        {
            if (!SelectedElements.Contains(element)) SelectedElements.Add(element);
        }

        RequestUpdate();
    }

    private int CntSelection()
    {
        SelectedElements
            .Where(e => !Children.Contains(e))
            .ToList()
            .ForEach(e => SelectedElements.Remove(e));
        RequestUpdate();
        return SelectedElements.Count;
    }

    private void ClrSelection()
    {
        SelectedElements.Clear();
        RequestUpdate();
    }

    private void DeleteSelectedElements()
    {
        try
        {
            var gop = new GroupOp();
            foreach (var view in SelectedElements)
            {
                gop.Add(new DeleteOp(this, view));
            }

            Operator.Execute(gop);
        }
        catch
        {
            // ignored
        }
    }

    private void ApplyTransformation()
    {
        if (SelectedElements.Count == 0) return;

        try
        {
            var gop = new GroupOp();
            foreach (var element in SelectedElements)
            {
                if (_previewBox.DeltaX != 0 || _previewBox.DeltaY != 0)
                {
                    var actPos = element.GetPosition();
                    gop.Add(new MoveOp(element,
                        new Point(actPos.X + _previewBox.DeltaX, actPos.Y + _previewBox.DeltaY)));
                }

                if (_previewBox.DeltaWidth != 0 || _previewBox.DeltaHeight != 0)
                {
                    var actSize = element.GetSize();
                    gop.Add(new ResizeOp(element,
                        new Size(Math.Max(5, actSize.Width + _previewBox.DeltaWidth),
                            Math.Max(5, actSize.Height + _previewBox.DeltaHeight))));
                }
            }

            Operator.Execute(gop);
        }
        catch
        {
            // ignored
        }
    }

    private void RequestUpdate() => _updateTimer.Start();

    private void UpdateSelectedComponents()
    {
        _updateTimer.Stop();
        if (CntSelection() == 0)
        {
            _selectedBox.Hide();
            _toolbar.Hide();
            return;
        }

        _selectedBox.Show(SelectingBounds);
        if (SelectedElements.Count == 1)
            _toolbar.Hide();
        else
            _toolbar.Show(SelectingBounds.BottomLeft);
    }
}

public static class ControlExtensions
{
    public static double GetCenter(this Control element, bool isHorizontal)
    {
        return isHorizontal
            ? Canvas.GetLeft(element) + element.Width / 2
            : Canvas.GetTop(element) + element.Height / 2;
    }

    internal static void SetPositon(this Control element, Point point)
    {
        Canvas.SetLeft(element, point.X);
        Canvas.SetTop(element, point.Y);
    }

    internal static Point GetPosition(this Control element) =>
        new(Canvas.GetLeft(element), Canvas.GetTop(element));

    internal static void SetSize(this Control element, Size size)
    {
        element.Width = size.Width;
        element.Height = size.Height;
    }

    internal static Size GetSize(this Control element) => new(element.Width, element.Height);

    internal static Rect GetBounds(this Control element) => new(Canvas.GetLeft(element),
        Canvas.GetTop(element), element.Width, element.Height);
}