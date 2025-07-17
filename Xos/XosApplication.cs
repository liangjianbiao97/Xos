using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public static readonly ServiceCollection Biulder = [];
    public static ServiceProvider Provider;

    public new static XosApplication? Current => Biulder.BuildServiceProvider().GetService<XosApplication>();

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            ConfigureServices();
            ConfigureServices(Biulder);
            LoadModules();
            Provider = Biulder.BuildServiceProvider();
            desktop.MainWindow = CreateShell(Biulder.BuildServiceProvider());
            desktop.ShutdownRequested += DesktopOnShutdownRequested;
            desktop.Exit+= DesktopOnExit;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DesktopOnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        var packer = Provider.GetService<ApplicationPacker>();
        packer?.Save();
    }

    private void DesktopOnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
    {

    }

    private void ConfigureServices()
    {
        Biulder.AddSingleton<ApplicationPacker>();
        Biulder.AddSingleton<XosApplication>(_ => this);
    }

    private void LoadModules()
    {
        ViewCollection = new ViewCollection(Biulder);
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