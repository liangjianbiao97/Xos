using Newtonsoft.Json;

namespace Xos.Service.ApplicationPackage;

public class PackageBase(string id)
{
    protected PackableItem Package { get; } = new(id);
    internal string Id => Package.Id;

    public virtual void Update() => throw new NotImplementedException();
    public virtual void ToObject(string data) => throw new NotImplementedException();

    public override string ToString() => JsonConvert.SerializeObject(Package.Data);
}