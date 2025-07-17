namespace Xos.Operation;

public class DeleteOp(XDesk desk, XView view) : IOperation
{
    public void Execute() => desk.Children.Remove(view);

    public void Undo() => desk.Children.Add(view);
    public void Redo() => Execute();
}