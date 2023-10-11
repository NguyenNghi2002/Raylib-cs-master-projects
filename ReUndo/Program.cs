using Engine;
using Engine.Texturepacker;
using Raylib_cs;
using System.Numerics;

new Game().Run();

public class Game : Core
{
    public override void Intitialize()
    {
        WindowWidth = 640;
        WindowHeight = 640;
        base.Intitialize();


        Scene = new MainMenu();
        var b = ContentManager.Load<TextureAtlas>("sprite", "sprite.xml");
        AddManager(new ImguiEntityManager());
    }
}

public class LineRenderer : Component
{
    public List<Vector2> points;
    public override void OnDebugRender()
    {
        Raylib.DrawLineStrip(points.ToArray(),points.Count,Color.RED);
    }

}