using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Xos.Module;
using Xos.Service.ApplicationPackage;

namespace Xos;

public abstract class XosApplication : Application
{
    internal static ViewCollection? ViewCollection;
    private readonly ServiceCollection _builder = [];
    public ServiceProvider Provider;

    public new static XosApplication? Current => Application.Current as XosApplication;

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            ConfigureServices(); //Xos Service Configure
            ConfigureServices(_builder);// User Service Configure
            LoadModules(); //ViewModules Loading
            Provider = _builder.BuildServiceProvider();
            desktop.MainWindow = CreateShell(Provider);
            desktop.ShutdownRequested += DesktopOnShutdownRequested;
        }

        base.OnFrameworkInitializationCompleted();
    }


    private void DesktopOnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
    {
        var packer = Provider.GetService<ApplicationPacker>();
        packer?.Save();
    }

    private void ConfigureServices()
    {
        var exePath = Environment.ProcessPath ?? throw new InvalidOperationException("无法获取当前进程路径");
        var path = $"{Path.GetDirectoryName(exePath)}/Runtime.xos";
        _builder.AddSingleton<ApplicationPacker>(_ => ApplicationPacker.CreateInstance(path));
        _builder.AddKeyedSingleton<IPackage, XPackage>("XPackage",
            (sp, key) => new XPackage(
                sp.GetRequiredService<ApplicationPacker>(),
                key.ToString()));
    }

    private void LoadModules()
    {
        ViewCollection = new ViewCollection(_builder);
        using var moduleloader = new ModuleLoader(ViewCollection);
        ConfigureViews(moduleloader);
        moduleloader.LoadAssembliesFromDirectory();
    }

    protected abstract void ConfigureViews(ModuleLoader moduleloader);
    protected abstract void ConfigureServices(ServiceCollection services);
    protected abstract Window CreateShell(ServiceProvider provider);

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove) BindingPlugins.DataValidators.Remove(plugin);
    }
}