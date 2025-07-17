using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Xos.Demo.Startup;
using Xos.Module;

namespace Xos.Demo;

public class App : XosApplication
{
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        
    }

    protected override void ConfigureViews(ModuleLoader moduleloader)
    {
    }
    protected override void ConfigureServices(ServiceCollection services)
    {
        services.AddTransient<ViewModel>();
        services.AddTransient<View>(sp => new View{ DataContext = sp.GetRequiredService<ViewModel>() });
    }

    protected override Window CreateShell(ServiceProvider provider)
    {
        return provider.GetRequiredService<View>();
    }
}