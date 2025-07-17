using System.Collections.Generic;

namespace Xos.Operation;

internal static class Operator
{
    private static readonly Stack<IOperation> UndoStack = new();
    private static readonly Stack<IOperation> RedoStack = new();

    public static void Execute(IOperation op)
    {
        op.Execute();
        UndoStack.Push(op);
        RedoStack.Clear();
    }

    public static void Undo()
    {
        if (UndoStack.Count <= 0) return;
        var op = UndoStack.Pop();
        op.Undo();
        RedoStack.Push(op);
    }

    public static void Redo()
    {
        if (RedoStack.Count <= 0) return;
        var op = RedoStack.Pop();
        op.Redo();
        UndoStack.Push(op);
    }
}