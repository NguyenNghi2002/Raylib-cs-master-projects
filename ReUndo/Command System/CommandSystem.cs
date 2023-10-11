using Engine;
public class CommandSystem : Component
{
    List<ICommandable> commands = new List<ICommandable>();
    public int commandIndex = 0;

    public bool SendRedoCommand()
    {
        if (commandIndex > commands.Count -1) return false;

        commands[commandIndex].Redo();
        commandIndex++;
        return true;
    }
    public bool SendUndoCommand()
    {
        if (commandIndex <= 0) return false;

        commandIndex--;
        commands[commandIndex].Undo();
        return true;

    }
    public void ExecuteCommand(ICommandable move)
    {
        if (commandIndex < commands.Count)
            commands.RemoveRange(commandIndex, commands.Count - commandIndex);

        commands.Add(move);
        move.Execute();
        commandIndex++;
    }
}
public interface ICommandable
{
    void Execute();
    void Undo();
    void Redo();
}
