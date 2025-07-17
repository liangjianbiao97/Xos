using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Xos.Operation;

namespace Xos.Component;

public class AlignmentToolbar(XDesk desk) : StackPanel
{
    private bool _isLayout;

    protected override void OnInitialized()
    {
        this.Orientation = Orientation.Horizontal;
        base.OnInitialized();
        CreateAlignmentButtons();
    }

    public void Show(Point point)
    {
        Canvas.SetLeft(this, point.X);
        Canvas.SetTop(this, point.Y);
        if (_isLayout) return;
        _isLayout = true;
        desk.Children.Add(this);
    }

    public void Hide()
    {
        if (!_isLayout) return;
        _isLayout = false;
        desk.Children.Remove(this);
    }

    private void CreateAlignmentButtons()
    {
        // 清除现有按钮
        Children.Clear();

        // 添加对齐按钮
        AddToolButton("左对齐", AlignLeft,
            "M0 0m16 0 48 0q16 0 16 16l0 992q0 16-16 16l-48 0q-16 0-16-16l0-992q0-16 16-16ZM184 156m40 0 420 0q40 0 40 40l0 180q0 40-40 40l-420 0q-40 0-40-40l0-180q0-40 40-40ZM184 608m40 0 760 0q40 0 40 40l0 180q0 40-40 40l-760 0q-40 0-40-40l0-180q0-40 40-40Z");
        AddToolButton("右对齐", AlignRight,
            "M944 0m16 0 48 0q16 0 16 16l0 992q0 16-16 16l-48 0q-16 0-16-16l0-992q0-16 16-16ZM340 156m40 0 420 0q40 0 40 40l0 180q0 40-40 40l-420 0q-40 0-40-40l0-180q0-40 40-40ZM0 608m40 0 760 0q40 0 40 40l0 180q0 40-40 40l-760 0q-40 0-40-40l0-180q0-40 40-40Z");
        AddToolButton("上对齐", AlignTop,
            "M0 0m16 0 992 0q16 0 16 16l0 48q0 16-16 16l-992 0q-16 0-16-16l0-48q0-16 16-16ZM608 184m40 0 180 0q40 0 40 40l0 420q0 40-40 40l-180 0q-40 0-40-40l0-420q0-40 40-40ZM156 184m40 0 180 0q40 0 40 40l0 760q0 40-40 40l-180 0q-40 0-40-40l0-760q0-40 40-40Z");
        AddToolButton("下对齐", AlignBottom,
            "M0 944m16 0 992 0q16 0 16 16l0 48q0 16-16 16l-992 0q-16 0-16-16l0-48q0-16 16-16ZM608 340m40 0 180 0q40 0 40 40l0 420q0 40-40 40l-180 0q-40 0-40-40l0-420q0-40 40-40ZM156 0m40 0 180 0q40 0 40 40l0 760q0 40-40 40l-180 0q-40 0-40-40l0-760q0-40 40-40Z");
        AddToolButton("水平居中", AlignCenterVertical,
            "M1024 472m0 16 0 48q0 16-16 16l-992 0q-16 0-16-16l0-48q0-16 16-16l992 0q16 0 16 16ZM416 432V44.16A44.16 44.16 0 00371.84 0H200.16A44.16 44.16 0 00156 44.16V432zM416 979.84V592H156v387.84a44.16 44.16 0 0044.16 44.16h171.68A44.16 44.16 0 00416 979.84zM868 786.75V592H608v194.75A45.25 45.25 0 00653.25 832h169.5A45.25 45.25 0 00868 786.75zM868 432V237.25A45.25 45.25 0 00822.75 192h-169.5A45.25 45.25 0 00608 237.25V432z");
        AddToolButton("垂直居中", AlignCenterHorizontal,
            "M472 0m16 0 48 0q16 0 16 16l0 992q0 16-16 16l-48 0q-16 0-16-16l0-992q0-16 16-16ZM432 608H44.16A44.16 44.16 0 000 652.16v171.68A44.16 44.16 0 0044.16 868H432zM979.84 608H592v260h387.84a44.16 44.16 0 0044.16-44.16V652.16A44.16 44.16 0 00979.84 608zM786.75 156H592v260h194.75A45.25 45.25 0 00832 370.75v-169.5A45.25 45.25 0 00786.75 156zM432 156H237.25A45.25 45.25 0 00192 201.25v169.5A45.25 45.25 0 00237.25 416H432z");

        // 分隔符
        Children.Add(new Border() { Margin = new Thickness(5), Width = 1, Background = Brushes.White });

        // 分布按钮
        AddToolButton("水平分布", DistributeHorizontal,
            "M0 1024m0-16 0-992q0-16 16-16l48 0q16 0 16 16l0 992q0 16-16 16l-48 0q-16 0-16-16ZM944 1024m0-16 0-992q0-16 16-16l48 0q16 0 16 16l0 992q0 16-16 16l-48 0q-16 0-16-16ZM312 92m49.61 0 300.78 0q49.61 0 49.61 49.61l0 740.78q0 49.61-49.61 49.61l-300.78 0q-49.61 0-49.61-49.61l0-740.78q0-49.61 49.61-49.61Z");
        AddToolButton("垂直分布", DistributeVertical,
            "M0 0m16 0 992 0q16 0 16 16l0 48q0 16-16 16l-992 0q-16 0-16-16l0-48q0-16 16-16ZM0 944m16 0 992 0q16 0 16 16l0 48q0 16-16 16l-992 0q-16 0-16-16l0-48q0-16 16-16ZM92 712m0-49.61 0-300.78q0-49.61 49.61-49.61l740.78 0q49.61 0 49.61 49.61l0 300.78q0 49.61-49.61 49.61l-740.78 0q-49.61 0-49.61-49.61Z");

        // 分隔符
        Children.Add(new Border() { Margin = new Thickness(5), Width = 1, Background = Brushes.White });

        // 网格控制
        var gridToggle = new ToggleButton
        {
            Content = "网格",
            Margin = new Thickness(2),
            Padding = new Thickness(5),
            MinWidth = 60
        };
        Children.Add(gridToggle);
        var snapToggle = new ToggleButton
        {
            Content = "对齐网格",
            Margin = new Thickness(2),
            Padding = new Thickness(5),
            MinWidth = 80
        };
        Children.Add(snapToggle);
    }

    private void AddToolButton(string tooltip, Action action, string pathData)
    {
        var button = new Button
        {
            Content = new PathIcon
            {
                Data = StreamGeometry.Parse(pathData),
            },
            Padding = new Thickness(5),
            Margin = new Thickness(2),
            Width = 30,
            Height = 30
        };
        ToolTip.SetTip(button, tooltip);
        button.Click += (s, e) => action?.Invoke();
        Children.Add(button);
    }

    // 对齐命令实现
    private void AlignLeft()
    {
        var lv = desk.SelectedElements.LastOrDefault();
        if (lv == null) return;
        var left = lv.X;
        var gop = new GroupOp();
        foreach (var view in desk.SelectedElements) gop.Add(new MoveOp(view, new Point(left, view.Y)));
        Operator.Execute(gop);
    }

    private void AlignRight()
    {
        var lv = desk.SelectedElements.LastOrDefault();
        if (lv == null) return;
        var right = lv.X + lv.W;
        var gop = new GroupOp();
        foreach (var view in desk.SelectedElements) gop.Add(new MoveOp(view, new Point(right - view.W, view.Y)));
        Operator.Execute(gop);
    }

    private void AlignTop()
    {
        var lv = desk.SelectedElements.LastOrDefault();
        if (lv == null) return;
        var top = lv.Y;
        var gop = new GroupOp();
        foreach (var view in desk.SelectedElements) gop.Add(new MoveOp(view, new Point(view.X, top)));
        Operator.Execute(gop);
    }

    private void AlignBottom()
    {
        var lv = desk.SelectedElements.LastOrDefault();
        if (lv == null) return;
        var bottom = lv.Y + lv.H;
        var gop = new GroupOp();
        foreach (var view in desk.SelectedElements) gop.Add(new MoveOp(view, new Point(view.X, bottom - view.H)));
        Operator.Execute(gop);
    }

    private void AlignCenterHorizontal()
    {
        var lv = desk.SelectedElements.LastOrDefault();
        if (lv == null) return;
        var centerX = lv.X + lv.W / 2;
        var gop = new GroupOp();
        foreach (var view in desk.SelectedElements) gop.Add(new MoveOp(view, new Point(centerX - view.W / 2, view.Y)));
        Operator.Execute(gop);
    }

    private void AlignCenterVertical()
    {
        var lv = desk.SelectedElements.LastOrDefault();
        if (lv == null) return;
        var centerY = lv.Y + lv.H / 2;
        var gop = new GroupOp();
        foreach (var view in desk.SelectedElements) gop.Add(new MoveOp(view, new Point(view.X, centerY - view.H / 2)));
        Operator.Execute(gop);
    }

    private void DistributeHorizontal()
    {
        var actualSpace = desk.SelectingBounds.Width;
        var sumW = desk.SelectedElements.Sum(v => v.W);
        var space = (actualSpace - sumW) / (desk.SelectedElements.Count - 1);
        if (space < 0) return;
        var left = desk.SelectingBounds.X;
        var gop = new GroupOp();
        foreach (var view in desk.SelectedElements.OrderBy(v => v.X))
        {
            gop.Add(new MoveOp(view, new Point(left, view.Y)));
            left += view.W + space;
        }

        Operator.Execute(gop);
    }

    private void DistributeVertical()
    {
        var actualSpace = desk.SelectingBounds.Height;
        var sumH = desk.SelectedElements.Sum(v => v.H);
        var space = (actualSpace - sumH) / (desk.SelectedElements.Count - 1);
        if (space < 0) return;
        var top = desk.SelectingBounds.Y;
        var gop = new GroupOp();
        foreach (var view in desk.SelectedElements.OrderBy(v => v.Y))
        {
            gop.Add(new MoveOp(view, new Point(view.X, top)));
            top += view.H + space;
        }

        Operator.Execute(gop);
    }
}