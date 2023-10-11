using Engine;
using Engine.SceneManager;
using Engine.TiledSharp;
using Engine.TiledSharp.Extension;
using Engine.Velcro;
using Engine.External;
using System.Numerics;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Utilities;
using Raylib_cs;
using Genbox.VelcroPhysics.Tools.Cutting.Simple;
using Genbox.VelcroPhysics.Tools.PolygonManipulation;
using Genbox.VelcroPhysics.Tools.Triangulation.TriangulationBase;

namespace Golf
{
#if true
    public class GameplayScene : Scene
    {

        public GameplayScene() : base("Gameplay", (int)(64 * 10), (int)(64 * 10), Color.GRAY, Color.BLACK)
        { }


        private World world = new World(0, 9.8f);
        private TmxMap tilemap;

        public override void OnLoad()
        {
        }
        public override void OnBegined()
        {
            
            //var oldDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(@"C:\Users\nghin\OneDrive\Desktop\Game Develop Tool\Tiled\tmxGolf");


            tilemap = new TmxMap("map1.tmx");

            TiledUtil.TryFindObjectGroup(tilemap,"Collider", out var collidersGroup);
            TiledUtil.TryFindObjectGroup(tilemap,"Entities", out var entitiesGroup);

            entitiesGroup.Objects.TryGetValue("Ball", out TmxObject? ballObj);
            entitiesGroup.Objects.TryGetValue("Hole", out TmxObject? holeObj);

            var map = CreateEntity("map");
            map.AddComponent(new TileMap(tilemap,"Base"));
            map.AddComponent(new TilemapBody("Collider",true));



            #region Player
            Vector2 playerSize = new Vector2((float)ballObj.Width, (float)ballObj.Height);
            Vector2 playerPosition = new Vector2((float)ballObj.X, (float)ballObj.Y);
            var player = CreateEntity("ball")
                .Move(playerPosition + playerSize.Half());

            player.AddComponent(new Ball());
            player.AddComponent(new BoxRenderer(playerSize.X, playerSize.Y, Color.RED));
            player.AddComponent(new SlingShotBodyController());
            player.AddComponent(new VRigidBody2D())
                .SetBodyType(BodyType.Dynamic);
            player.AddComponent(new VCollisionPolygon(PolygonUtils.CreateRoundedRectangle(playerSize.X, playerSize.Y, 5f, 5f, 1)))
                .SetRestitution(0.7f);

            //player.AddComponent(new VCollisionPolygon(SimpleCombiner.PolygonizeTriangles(Triangulate.ConvexPartition(PolygonUtils.CreateRectangle(10,10),TriangulationAlgorithm.Earclip,true,0.001f))));
            #endregion


            var hole = CreateEntity("hole")
                .MoveTo(holeObj.X, holeObj.Y);
            hole.AddComponent(new CircleRenderer());
            hole.AddComponent(new BallTarget());


            var uiCanvas = CreateEntity("uiCanvas");
            uiCanvas.AddComponent(new UICanvas());
            CreateEntity("Mouse")
                .AddComponent<FollowCursor>()
                .AddComponent<CircleRenderer>()
                ;

        }

        public override void OnUnload()
        {
            world = null;
            tilemap.Unload();
        }
        public override void OnRender()
        {
            base.OnRender();
            Raylib.DrawFPS(10,10);
        }


    }

    public class BallTarget : Component,IUpdatable
    {
        private Ball ball;

        bool isTouched;
        public float touchRange = 50;

        public int UpdateOrder { get; set; }

        public override void OnAddedToEntity()
        {
            if (Scene.TryFindComponent<Ball>(out ball))
            {
            }
        }

        public void Update()
        {
            if (isTouched)
            {
                if (!CheckTouch())
                {
                    Console.WriteLine("Left");
                    isTouched = !isTouched;
                    Scene.GetOrCreateSceneComponent<GameManager>().WinGame();
                }
            }
            else
            {
                if (CheckTouch())
                {
                    Console.WriteLine("go in");
                    isTouched = !isTouched; 
                    
                }

            }
        }

        bool CheckTouch()
        {
            var dis = Vector2.Distance(ball.Transform.Position2, this.Transform.Position2);
            return dis <= touchRange;
        }
        public override void OnDebugRender()
        {
            Raylib.DrawCircleV(Transform.Position2,touchRange,new Color(255,0,0,100));
        }
    }


#endif

}