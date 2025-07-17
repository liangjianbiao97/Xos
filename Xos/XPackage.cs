using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xos.Module;
using Xos.Service.ApplicationPackage;

namespace Xos;

public class XPackage(ApplicationPacker packer, string id) : PackageBase(packer, id)
{
    private XDesk? _desk;

    private List<XProperty> Properties
    {
        get => (List<XProperty>)(Package.Data ??= new List<XProperty>());
        set => Package.Data = value;
    }

    public override void FillTo<T>(T desk)
    {
        if (desk is XDesk xDesk) _desk = xDesk;
        else return;
        foreach (var xProp in  Properties)
        {
            var info = XosApplication.ViewCollection!.Infos.FirstOrDefault(v => v .ViewName == xProp.ViewName);
            if (info == null) continue;
            var view = new XView(info, default);
             _desk.Children.Add(view);
            view.ApplySettings(xProp.Settings);
        }
        Properties.Clear();
    }

    public override void Update()
    {
        if (_desk == null) return;
        Properties.Clear();
        foreach (var view in _desk.Children.OfType<XView>())
        {
            Properties.Add(new XProperty { ViewName = view.ViewName, Settings = view.GetSettings() });
        }
    }

    public override void ToObject(string data)
    {
        try
        {
            Properties = JsonConvert.DeserializeObject<List<XProperty>>(data) ?? [];
        }
        catch
        {
            Properties = [];
        }
    }
}