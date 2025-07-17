using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Newtonsoft.Json;
using Xos.Module;
using Xos.Property;

namespace Xos;

public class XView : ContentControl
{
    private readonly ViewModelBase _viewModel;
    private Contour _contour;
    private Permission _permission;

    #region Properties

    public string ViewName;

    public int Level
    {
        get => _permission.Level;
        set => _permission.Level = value;
    }

    public double X
    {
        get => _contour.X;
        set
        {
            _contour.X = value;
            Canvas.SetLeft(this, value);
        }
    }

    public double Y
    {
        get => _contour.Y;
        set
        {
            _contour.Y = value;
            Canvas.SetTop(this, value);
        }
    }

    public int Z
    {
        get => _contour.Z;
        set
        {
            _contour.Z = value;
            
        }
    }

    public double W
    {
        get => _contour.Width;
        set
        {
            _contour.Width = value;
            Width = value;
        }
    }

    public double H
    {
        get => _contour.Height;
        set
        {
            _contour.Height = value;
            Height = value;
        }
    }

    #endregion

    public XView(ViewInfo viewInfo, Point position)
    {
        if (viewInfo.ViewName == null || viewInfo.GetView == null) throw new ArgumentNullException();
        var view = viewInfo.GetView.Invoke();
        _viewModel = view.DataContext as ViewModelBase ?? throw new ArgumentNullException();
        ViewName = viewInfo.ViewName;
        Content = view;
        X = position.X;
        Y = position.Y;
        W = viewInfo.InitWidth;
        H = viewInfo.InitHeight;
    }

    internal Dictionary<string, string> GetSettings()
    {
        var settings = new Dictionary<string, string>
        {
            ["轮廓"] = JsonConvert.SerializeObject(_contour, Formatting.Indented),
            ["安全"] = JsonConvert.SerializeObject(_permission, Formatting.Indented),
        };
        _viewModel.GetSettings(settings);
        return settings;
    }

    internal void ApplySettings(Dictionary<string, string> settings)
    {
        _contour = JsonConvert.DeserializeObject<Contour>(settings["轮廓"]);
        X = _contour.X;
        Y = _contour.Y;
        Z = _contour.Z;
        W = _contour.Width;
        H = _contour.Height;
        _permission = JsonConvert.DeserializeObject<Permission>(settings["安全"]);
        Level = _permission.Level;
        _viewModel.ApplySetting(settings);
    }
}