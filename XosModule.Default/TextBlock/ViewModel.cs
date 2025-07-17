using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Xos.Module;

namespace XosModule.Default.TextBlock;

public partial class ViewModel : ViewModelBase
{
    [ObservableProperty] private string _text = "Welcome to Xos!";

    public override void GetSettings(Dictionary<string, string> settings)
    {
        settings["Text"] = Text;
    }

    public override void ApplySetting(Dictionary<string, string> settings)
    {
        if (settings.TryGetValue("Text", out var text)) Text = text;
    }
}