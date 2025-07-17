using System.Runtime.InteropServices.JavaScript;
using Newtonsoft.Json;

namespace Xos.Service.ApplicationPackage;

public class ApplicationPacker
{
    private readonly string _path;

    private List<PackageBase> Packages { get; set; } = [];
    private readonly Dictionary<string, string> _data;

    private Action? OnPreviewSave { get; set; }

    public ApplicationPacker()
    {
        // 方法1：使用Process获取（最准确）
        var exePath = Environment.ProcessPath ?? throw new InvalidOperationException("无法获取当前进程路径");
        var path = $"{Path.GetDirectoryName(exePath)}/Runtime.xos";
        _path = path;
        var data = PackageZipper.DecompressStringFromFile(_path);
        _data = JsonConvert.DeserializeObject<Dictionary<string, string>>(data)??[];
    }

    public void Binding(PackageBase package)
    {
        if (_data.TryGetValue(package.Id, out var value))
        {
            package.ToObject(value);
            _data.Remove(package.Id);
        }
        Packages.Add(package);
        OnPreviewSave += package.Update;
    }
    
    
    public void Save()
    {
        OnPreviewSave?.Invoke();
        var dict = new Dictionary<string, string>();
        foreach (var pkg in Packages) dict[pkg.Id] = dict.ToString()!;
        var data = JsonConvert.SerializeObject(dict);
        PackageZipper.CompressStringToFile(data, _path);
    }
}