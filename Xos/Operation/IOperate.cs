namespace Xos.Operation;

internal interface IOperation
{
    void Execute();
    void Undo();
    void Redo();
}