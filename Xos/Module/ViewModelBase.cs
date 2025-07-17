using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Xos.Module;

public abstract class ViewModelBase : ObservableObject
{
    public abstract void GetSettings(Dictionary<string,string> settings);
    public abstract void ApplySetting(Dictionary<string,string> settings);
}