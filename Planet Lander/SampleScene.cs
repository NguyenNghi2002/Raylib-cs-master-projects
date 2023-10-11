using Engine;
using Engine.DefaultComponents.Render.Primitive;
using Engine.SceneManager;
using Engine.UI;
using ImGuiNET;
using Raylib_cs;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

internal class SampleScene : Scene
{
    public SampleScene(string sceneName, int width, int height, Color backgroundColor, Color letterBoxClearColor)
        : base(sceneName, width, height, backgroundColor, letterBoxClearColor)
    {
    }

    public override void OnLoad()
    {
        Console.WriteLine();
        //Load Your asset using 
        ContentManager.Load<rTexture>("moon",".asset\\moon.png");
        ContentManager.Load<rTexture>("ball",".asset\\ball.png");
        ContentManager.Load<rTexture>("trail_01",".asset\\trail01.png");
    }

    public override void OnBegined()
    {
        
        Filter = TextureFilter.TEXTURE_FILTER_BILINEAR;

        var position = ViewPortScale / 2f;
        var radius = MathF.Min(ViewPortWidth, ViewPortHeight) / 4f;


        ///MUST BE ON TOP
        var GameManager = CreateEntity("Manager")
            .AddComponent<GameSceneManager>()
            .AddComponent<CameraController>()
            .AddComponent<UICanvas>()
            .Entity.SetProcessOrder(int.MaxValue)
            ;



        //CreatePlanet(this,position,40);

        /////////////////////////////////////////////////////////
        var player = CreateEntity("Player", Vector2.UnitY)
                .AddComponent<Ball>()
                .AddComponent<Trail>(new())
                .Entity
                .SetProcessOrder(1);
        ;
        //--------------------//
        {
            CreateChildEntity(player, "player-image")
                //.AddComponent<CircleRenderer>(new(10, Color.GREEN))
                .AddComponent<SpriteRenderer>(new(ContentManager.Get<rTexture>("ball")))
                .SetRenderOrder(10)
                ;
        }
        //--------------------//
        /////////////////////////////////////////////////////////
        ///
    }

    public static Entity CreatePlanet(Scene scene,Vector2 position,float radius,out Planet planet)
    {
        /////////////////////////////////////////////////////////
        var planet_root = scene.CreateEntity("Planet", position)
                .AddComponent<Planet>(new(radius),out planet)
                .AddComponent<Shakable>()
                .AddComponent<Pulsable>()
                .Entity
            ;
        //--------------------//
        var color = Raylib.ColorFromNormalized(new Vector4(Random.Shared.NextSingle(), Random.Shared.NextSingle(), Random.Shared.NextSingle(), 1f));

        scene.CreateChildEntity(planet_root, "planet-image", Vector2.Zero, false)
            //.AddComponent<CircleRenderer>(new(radius, color))
            .AddComponent<SpriteRenderer>(new(ContentManager.Get<rTexture>("moon")));
            ;
        var planet_center =
        scene.CreateChildEntity(planet_root, "planet-center")
            ;

        scene.CreateChildEntity(planet_center, "planet-slot")
            //.AddComponent<RingRenderer>(new(10,10-4, Color.LIGHTGRAY))

            ;
        //--------------------//
        /////////////////////////////////////////////////////////
        Debug.Assert(planet_root != null);
        return planet_root;
        
    }
}
