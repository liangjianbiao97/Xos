using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xos.Service.ApplicationPackage;

namespace Xos;

public class XPackage(string id) : PackageBase(id)
{
    private readonly XDesk _desk;

    private List<XProperty> Properties
    {
        get => (List<XProperty>)Package.Data;
        set => Package.Data = value;
    }

    public XPackage(XDesk desk, string id) : this(id)
    {
        _desk = desk;
        XosApplication.Provider
            .GetService<ApplicationPacker>()?
            .Binding(this);
    }

    public override void Update()
    {
        Properties.Clear();
        foreach (var view in _desk.Children.OfType<XView>())
        {
            if (view.Name is null) continue;
            Properties.Add(new XProperty { ViewName = view.Name, Settings = view.GetSettings() });
        }
    }

    public override void ToObject(string data)
    {
        Properties = JsonConvert.DeserializeObject<List<XProperty>>(data) ?? [];
    }
}