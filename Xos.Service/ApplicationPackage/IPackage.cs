namespace Xos.Service.ApplicationPackage;

public interface IPackage
{
    string Id { get; }
    void FillTo<T>( T container);
    void Update();
    void ToObject(string data);
    string ToString();
}