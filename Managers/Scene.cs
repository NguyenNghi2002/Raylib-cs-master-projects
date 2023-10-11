using Raylib_cs;

public class Scene
{
    public string Name { get; set; }

    internal SceneManager Manager;
    internal SceneState State;

    public override string ToString()
    {
        return Name;
    }

    internal void LoadContent()
    {
        for (int i = 0; i < 10; i++)
        {
            //Task.Delay(1).Wait();
            Console.Write(i);
        }
            Console.WriteLine();
    }
    internal void Start()
    {
    }
    internal void Draw()
    {
        Raylib.DrawText(Name,30,30,20,Color.RED);
        Raylib.DrawCircleV(Raylib.GetMousePosition(), 2, Color.RED);
    }
    internal void Update()
    {
    }
    
}
