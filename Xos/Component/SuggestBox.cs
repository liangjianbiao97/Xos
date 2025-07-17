using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Xos.Operation;


namespace Xos.Component;

public class SuggestBox
{
    private readonly AutoCompleteBox _box;
    public bool IsActivated;
    private readonly XDesk _desk;
    private Point _position;

    public SuggestBox(XDesk desk)
    {
        _desk = desk;
        _box = new AutoCompleteBox()
        {
            ItemsSource = XosApplication.ViewCollection!.Infos.Select(info => info.ViewName),
            Focusable = true
        };
        _box.KeyDown += BoxOnKeyDown;
    }

    private void BoxOnKeyDown(object? sender, KeyEventArgs e)
    {
        switch (e)
        {
            case { Key: Key.Enter }:
                var info = XosApplication.ViewCollection!.Infos.FirstOrDefault(x => x.ViewName == _box.Text);
                if (info != null) Operator.Execute(new CreateOp(_desk, info, _position));
                Hide();
                e.Handled = true;
                break;
            case { Key: Key.Escape }:
                Hide();
                e.Handled = true;
                break;
        }
    }

    internal void Show(Point point)
    {
        _position = point;
        _desk.Children.Add(_box);
        Canvas.SetLeft(_box, point.X);
        Canvas.SetTop(_box, point.Y);
        _box.Text = string.Empty;
        _box.Focus();
        IsActivated = true;
        
    }

    internal void Hide()
    {
        _box.Text = string.Empty;
        _desk.Children.Remove(_box);
        _desk.Focus();
        IsActivated = false;
    }
}