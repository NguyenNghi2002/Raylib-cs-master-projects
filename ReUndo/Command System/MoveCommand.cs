using Engine;
using System.Numerics;

public class MoveCommand : ICommandable
{
    public Vector2 PrevPosition;
    public Vector2 Direction;
    public float MoveDistance;

    public Transformation Context;
    public MoveCommand(Transformation context,Vector2 direction,float movedistance)
    {
        this.Context = context;
        this.PrevPosition = context.Position2;
        this.Direction = direction;
        this.MoveDistance = movedistance;
    }


    void ICommandable.Execute()
    {
        Context.Position2 += Direction * MoveDistance;
    }

    void ICommandable.Redo()
    {
        Context.Position2 += Direction * MoveDistance;
    }

    void ICommandable.Undo()
    {
        Context.Position2 -= Direction * MoveDistance;
    }
}