using Avalonia;

namespace Xos.Operation;

public class MoveOp(XView view, Point point) : IOperation
{
    private readonly Point _oldPoint = new(view.X, view.Y);

    public void Execute()
    {
        view.X = point.X;
        view.Y = point.Y;
    }

    public void Undo()
    {
        view.X = _oldPoint.X;
        view.Y = _oldPoint.Y;
    }

    public void Redo() => Execute();
}