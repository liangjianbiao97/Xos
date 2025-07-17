using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Xos.Module;

public class ModuleLoader : IDisposable
{
    private readonly string _directoryPath;
    private readonly ViewCollection _views;

    public ModuleLoader(ViewCollection views)
    {
        // 方法1：使用Process获取（最准确）
        var exePath = Environment.ProcessPath ?? throw new InvalidOperationException("无法获取当前进程路径");
        var path = $"{Path.GetDirectoryName(exePath)}";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        _directoryPath = path;
        _views = views;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public void LoadAssembly<T>() where T : IModule, new()
    {
        new T().ConfigureViews(_views);
    }

    /// <summary>
    ///     加载目录中的所有程序集
    /// </summary>
    internal void LoadAssembliesFromDirectory()
    {
        var assemblyFiles = Directory.GetFiles(_directoryPath, "XosModule.*.dll");

        foreach (var file in assemblyFiles)
            try
            {
                var ab = Assembly.LoadFrom(file);
                foreach (var cfg in ab.GetTypes().Where(x => typeof(IModule).IsAssignableFrom(x) && !x.IsAbstract))
                {
                    var c = Activator.CreateInstance(cfg) as IModule;
                    c?.ConfigureViews(_views);
                }

                Console.WriteLine($"已加载: {Path.GetFileName(file)}");
            }
            catch (BadImageFormatException)
            {
                // 跳过非托管DLL或无效程序集
                Console.WriteLine($"跳过非程序集文件: {Path.GetFileName(file)}");
            }
            catch (FileLoadException ex)
            {
                Console.WriteLine($"加载失败: {Path.GetFileName(file)} - {ex.Message}");
            }
    }
}