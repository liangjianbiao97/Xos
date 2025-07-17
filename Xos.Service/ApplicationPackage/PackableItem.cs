using Newtonsoft.Json;

namespace Xos.Service.ApplicationPackage;

public class PackableItem(string id)
{
    public string Id { get; set; } = id;
    public object? Data { get; set; }

    public T? GetObject<T>()
    {
        if (Data == null) return default;
        return (T)Data;
    }
}