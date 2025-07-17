using Xos.Module;

namespace XosModule.Default;

public class DefaultModule : IModule
{
    public void ConfigureViews(ViewCollection collection)
    {
        collection.RegisterView<TextBlock.View, TextBlock.ViewModel>(group: ModuleGroup.System, initWidth: 200,
            initHeight: 20);
    }
}