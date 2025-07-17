using Newtonsoft.Json;

namespace Xos.Service.ApplicationPackage;

public class PackageBase : IPackage
{
    protected PackageBase(ApplicationPacker packer, string id)
    {
        Package = new PackableItem(id);
        packer.Binding(this);
    }

    protected PackableItem Package { get; }
    public string Id => Package.Id;

    public virtual void FillTo<T>(T container) => throw new NotImplementedException();
    public virtual void Update() => throw new NotImplementedException();
    public virtual void ToObject(string data) => throw new NotImplementedException();
    public override string ToString() => JsonConvert.SerializeObject(Package.Data);
}