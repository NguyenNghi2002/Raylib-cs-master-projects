using Engine;
using Raylib_cs;
using System.Numerics;

new GameCore().Run();

public class GameCore : Engine.Core
{

    VectorInt2 Samsungs22PlusRes = new VectorInt2(1080, 2340);
    VectorInt2 Window = new VectorInt2(1920/2, 1080/2);
    public override void Intitialize()
    {
        var windowHints = ConfigFlags.FLAG_WINDOW_RESIZABLE;
        Raylib.SetConfigFlags(windowHints);

        var currDecive = Window;

        WindowWidth = currDecive.Y;
        WindowHeight = currDecive.X;
        base.Intitialize();


        var ratio = 1280f / 720f;
        Scene = new SampleScene("Sample", 720, 1280, Color.DARKBLUE, Color.BLACK);
        //Scene.Scaling = Engine.SceneManager.DesignScaling.Truncate;

        Managers.Add(new ImguiEntityManager());
    }
}
