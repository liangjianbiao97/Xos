using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Xos.Module;

public class ViewCollection(ServiceCollection service)
{
    internal List<ViewInfo> Infos = [];

    public void RegisterView<TV, TVm>(ModuleGroup group, double initWidth = 120, double initHeight = 40)
        where TV : UserControl, new()
        where TVm : ViewModelBase
    {
        service.AddTransient<TVm>();
        service.AddTransient(f => new TV { DataContext = f.GetRequiredService<TVm>() });
        try
        {
            var viewName = typeof(TV).FullName?.Split('.')[^2];
            if (Infos.Any(i => i.ViewName == viewName)) throw new ArgumentException("View name already exists");
            Infos.Add(new ViewInfo
            {
                Group = group,
                ViewName = viewName,
                InitWidth = initWidth,
                InitHeight = initHeight,
                GetView = service.BuildServiceProvider().GetRequiredService<TV>
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}