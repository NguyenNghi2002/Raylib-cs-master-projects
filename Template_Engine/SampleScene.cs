using Engine;
using Engine.DefaultComponents.Render.Primitive;
using Engine.SceneManager;
using Raylib_cs;
using System.Numerics;

internal class PlayScene : Scene
{
    public PlayScene(string sceneName, int width, int height, Color backgroundColor, Color letterBoxClearColor) 
        : base(sceneName, width, height, backgroundColor, letterBoxClearColor)
    {
    }

    public override void OnLoad()
    {
        Console.WriteLine();
        //Load Your asset using 
        // ContentManager.Load();
    }

    public override void OnBegined()
    {
        Filter = TextureFilter.TEXTURE_FILTER_BILINEAR;

        var position = ViewPortScale / 2f;
        var radius = MathF.Min(ViewPortWidth, ViewPortHeight) / 4f;
        CreateEntity("Circle", position)
            .AddComponent<CircleRenderer>(new (radius,Color.WHITE)) // Add Render Component




            ; 

        // Add your Entity
    }
}
