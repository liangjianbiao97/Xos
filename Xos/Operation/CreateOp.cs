using Avalonia;
using Xos.Module;

namespace Xos.Operation;

public class CreateOp(XDesk desk, ViewInfo info, Point position) : IOperation
{
    private readonly XView _view = new(info, position);

    public void Execute() => desk.Children.Add(_view);

    public void Undo() => desk.Children.Remove(_view);

    public void Redo() => Execute();
}