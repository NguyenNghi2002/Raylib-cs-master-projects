using Engine;
using Engine.AI.FSM;
using Engine.SceneManager;
using Engine.UI;
using Raylib_cs;

public class GameManager : SceneComponent
{
    public static GameManager instance;
    public StateMachine<GameManager> GameStateMachine;

    public event Action OnWinned;

    public UICanvas UI;
    public TextButton OptionBtt;

    public Dialog OptionDialog ;
    public Dialog WinDialog ;

    internal Player player;
    internal List<Door> doors;

    public override void OnAddedToScene()
    {
         instance ??= this;

        var skin = Skin.DefaultSkin;
        (skin.Get<TextButtonStyle>().Up as PrimitiveDrawable).Color = new Color(0, 0, 255, 100);
        (skin.Get<TextButtonStyle>().Up as PrimitiveDrawable).OutlineColor = new Color(0, 0, 255, 100);

        var winBg = skin.Get<WindowStyle>().Background as PrimitiveDrawable;
        winBg.Color = new Color(0, 0, 255, 100);

        OptionBtt = new TextButton(" x ", skin.Get<TextButtonStyle>());
        OptionBtt.OnChanged += isChecked =>
        {
            if (isChecked)
                GameStateMachine.ChangeState<GameState.Pause>();
            else GameStateMachine.ChangeState<GameState.Resume>();
        };
        
        OptionBtt.ProgrammaticChangeEvents = true;
        OptionDialog = new Dialog("Option", skin);
        OptionDialog.SetMovable(false).Pad(10);

        var listBox = new SelectBox<string>(skin);
        listBox.OnChanged += (s) =>
        {
            Console.WriteLine(s);
        };

        listBox.SetItems("item1", "item2", "item3", "item4", "item1", "item2", "item3", "item4", "item1", "item2", "item3", "item4", "item1", "item2", "item3", "item4", "item1", "item2", "item3", "item4", "item1", "item2", "item3", "item4", "item1", "item2", "item3", "item4", "item1", "item2", "item3", "item4");

        //var imgBtt = new ImageButton(ImageButtonStyle.CreateRaylib(100, 100, ContentManager.Get<TextureAtlas>("sprite").Sprites["Knight 64x64"]));
        //imgBtt.Pad(30);
        if (Scene.TryFindComponent<UICanvas>(out UI))
        {

            var tb = new Table()
                .SetFillParent(true)
                .Top().Right()
                //.DebugAll()
                ;
            //tb.Add(listBox).Pad(20);
            //tb.Add(imgBtt);
            tb.Add(OptionBtt).Pad(20);
            UI.Stage.AddElement(tb);

        }
        if (Scene.TryFindComponent<Player>(out player))
        {
            
        }

        var raylibStyle = TextButtonStyle.CreateRaylib();

        


        GameStateMachine = new StateMachine<GameManager>(this, new GameState.Resume());
        GameStateMachine.AddState(new GameState.Pause());
        GameStateMachine.AddState(new GameState.Winning());

        GameStateMachine.OnStateChanged += () =>
        {
            Console.WriteLine(GameStateMachine.CurrentState);
        };
    }


    public override void OnRemoveFromScene()
    {
        OnWinned = null;
        UI = null;
        OptionBtt = null;
        OptionDialog = null;
        WinDialog = null;
    }

    public override void Update()
    {
        GameStateMachine?.Update(Time.DeltaTime);
    }
}
