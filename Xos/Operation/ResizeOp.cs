using Avalonia;

namespace Xos.Operation;

public class ResizeOp(XView view, Size size) : IOperation
{
    private readonly Size _oldSize = new Size(view.Width, view.Height);

    public void Execute()
    {
        view.W = size.Width;
        view.H = size.Height;
    }

    public void Undo()
    {
        view.W = size.Width;
        view.H = size.Height;
    }

    public void Redo() => Execute();
}