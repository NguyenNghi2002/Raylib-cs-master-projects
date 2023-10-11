using Engine;
using Engine.External;
using Engine.SceneManager;
using Engine.Texturepacker;
using Engine.TiledSharp;
using Engine.TiledSharp.Extension;
using Engine.UI;
using Raylib_cs;

public class MainMenu : Scene
{
    public MainMenu() : base("menu",64*5,64*5, new Color(13, 24, 33), Color.BLACK) { }
    public override void OnBegined()
    {
        base.OnBegined();


        var menu = new Table()
            .SetFillParent(true)
            .Center()
            ;


        menu.Add(new Label("Re-Undo"))
            .SetPadBottom(30)
            .SetAlign(Align.Center);

        menu.Row();

        //Table of buttons
        var bttTable = new Table();
        menu.Add(bttTable);

        var bttStyle = TextButtonStyle.CreateRaylib();
        var quitStyle = TextButtonStyle.CreateRaylib();
        ((PrimitiveDrawable)quitStyle.Over).Color = Color.GOLD;
        ((PrimitiveDrawable)quitStyle.Over).OutlineColor = Color.DARKPURPLE;

        ((PrimitiveDrawable)quitStyle.Down).Color = Color.RED;
        ((PrimitiveDrawable)quitStyle.Down).OutlineColor = Color.PINK;


        var playBtt = new TextButton("Play", bttStyle)
            .AddLeftMouseListener(
                _ => Core.StartTransition(
                    new FadeTransition(() => new Gameplay(0))
                    )
            );
        var quitBtt = new TextButton("Quit", quitStyle)
            .AddLeftMouseListener(
                (btt) => Core.Instance.ExitApp()
            ) ;

        bttTable.Add(playBtt)
            .Pad(5);
        bttTable.Add(new TextButton("Setting", bttStyle))
            .Pad(5);

        bttTable.Row();

        bttTable.Add(new TextButton("About", bttStyle))
            .Pad(5);
        bttTable.Add(quitBtt)
            .Pad(5);

        CreateEntity("uis")
            .AddComponent<UICanvas>()
            .Stage
                .AddElement(menu);
            ;
    }
}
public class Gameplay : Scene
{
    
    protected TmxMap map;
    public int MoveCount = 10;
    public readonly int Level;
    public Gameplay(int level) 
        : base($"level{level}", 64 * 5 , 64 * 5,  new Color(13,24,33), Color.BLACK)
    {
        Level = level;
    }
    public override void OnBegined()
    {
        map = new TmxMap(@$"C:\Users\nghin\OneDrive\Desktop\Game Develop Tool\Tiled\Reundo\level{Level}.tmx");

        ContentManager.TryGet<TextureAtlas>("sprite", out TextureAtlas found);
        Console.WriteLine(found);

        map.TryFindObjectGroup("entities",out var entitiesObjs);
        var startObj = entitiesObjs.Objects["start"];
        var endObj = entitiesObjs.Objects["end"];
        //
        var keysObjs = from o in entitiesObjs.Objects 
                      where o.Class.ToLower() == "key" 
                      select o;
        var doorsObjs = from o in entitiesObjs.Objects
                      where o.Class.ToLower() == "door"
                      select o;




        CreateEntity("map")
            .AddComponent(new TileMap(map,"wall","blocker"))
            .AddComponent(new Grid("wall"))
            ;

        CreateEntity("goal")
            .MoveTo(endObj.X,endObj.Y)
            .AddComponent(new Goal())
            .AddComponent(new CircleRenderer(1,Color.RED))
            ;

        if(map.Properties.TryGetValue("moveCount",out var mc) 
            && int.TryParse(mc,out var m))
            MoveCount = m;
            
        CreateEntity("Player")
            .MoveTo(startObj.X, startObj.Y)

            // Logic
            .AddComponent<Player>()
                .SetMoveCount(MoveCount)
            .AddComponent<TurnBaseController>()
            .AddComponent<CommandSystem>()
            // Rendering
            .AddComponent<CircleRenderer>()
            ;


        foreach (var k in keysObjs)
        {
            var id = int.Parse(k.Properties["keyID"]);
            CreateEntity("key")
            .MoveTo(k.X, k.Y)
            .AddComponent(new Key(id))
            .AddComponent(new CircleRenderer(5, Color.YELLOW))
            
            ;
        }

        //search door
        foreach (var d in doorsObjs)
        {
            var id = int.Parse(d.Properties["doorID"]);
            CreateEntity("door")
                .MoveTo(d.X, d.Y)
            .AddComponent(new Door(id))
            .AddComponent(new CircleRenderer(10, Color.GREEN))
            ;
        }

        // UI
        CreateEntity("uicanvas")
            .AddComponent<UICanvas>();

    }
    public override void OnUnload()
    {
        map.Unload();
    }
}
