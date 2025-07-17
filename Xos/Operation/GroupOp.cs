using System;
using System.Collections.Generic;

namespace Xos.Operation;

public class GroupOp:IOperation
{
    private readonly Stack<IOperation> _ops = new (); 
    public void Execute()
    {
       foreach (var op in _ops) op.Execute();
    }

    public void Undo()
    {
        foreach (var op in _ops) op.Undo();
    }

    public void Redo()
    {
        foreach (var op in _ops) op.Redo();
    }

    internal void Add(IOperation op)
    {
        _ops.Push(op);
    }
}