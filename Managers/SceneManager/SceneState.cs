public abstract class SceneState : IState<SceneManager>
{
    public virtual void OnStart(SceneManager context)
    {
        Console.WriteLine($"enter {context.current} [{this}]");
    }

    public virtual void OnEnd(SceneManager context)
    {
        Console.WriteLine($"exit {context.current} [{this}]");
    }

    public virtual void OnProcessing(SceneManager context)
    {
        context?.current?.Update();
    }

    public virtual void OnRendering(SceneManager context)
    {
        context?.current?.Draw();
    }
    public sealed override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public sealed override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public sealed override string ToString()
    {
        return base.ToString();
    }
}
