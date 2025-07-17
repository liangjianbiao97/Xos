using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Xos.Service.ApplicationPackage;

namespace Xos.Component;

internal class ViewsLoader
{
    private readonly ServiceProvider _provider = XosApplication.Biulder.BuildServiceProvider();
    private readonly List<XViewData> _datas = [];
    
    public ViewsLoader(XDesk desk)
    {
        var packer = _provider.GetRequiredService<ApplicationPacker>();
    }
    
    



}

internal struct XViewData
{
   public string Name  { get; set; } 
   public Dictionary<string, string> Settings { get; set; }  
}
