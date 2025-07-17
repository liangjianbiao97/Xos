using System.Runtime.InteropServices.JavaScript;
using Newtonsoft.Json;

namespace Xos.Service.ApplicationPackage;

public class ApplicationPacker
{
    private readonly string _path;
    private static ApplicationPacker? _packer;
    private List<IPackage> Packages { get; set; } = [];
    private readonly Dictionary<string, string> _data;

    private Action? OnPreviewSave { get; set; }

    private ApplicationPacker(string path)
    {
        _path = path;
        var data = PackageZipper.DecompressStringFromFile(_path);
        _data = JsonConvert.DeserializeObject<Dictionary<string, string>>(data) ?? [];
    }

    public static ApplicationPacker CreateInstance(string path) => _packer ??= new ApplicationPacker(path);


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
        foreach (var pkg in Packages) dict[pkg.Id] = pkg.ToString();
        var data = JsonConvert.SerializeObject(dict);
        PackageZipper.CompressStringToFile(data, _path);
    }
}