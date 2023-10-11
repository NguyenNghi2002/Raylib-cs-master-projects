using Engine;
using Engine.SceneManager;
using Engine.UI;
using ImGuiNET;
using Raylib_cs;
using System.Collections;
using System.Diagnostics;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

public class GameSceneManager : Component,ICustomInspectorImgui
{
    public const uint ShowPlanetCount = 2;
    public static GameSceneManager Instance;

    public float BallFloatSpeed = 200f;

    public Queue<Planet> planetNextQueue = new Queue<Planet>();

    public Ball Player;
    public Planet CurrentPlanet;

    public uint Score = 0;
    public Label ScoreLabel_UI;

    public override void OnAddedToEntity()
    {
        //Setup UI
        if (Entity.TryGetComponent<UICanvas>(out var ui))
        {
            Table mainTable = new Table()
                .SetFillParent(true)
                .Top().Right()
                .DebugCell()
                .Pad(10)
                ;

            Table scoreTable = new Table()
                .SetFillParent(true)
                .Top()
                .PadTop(10)

                ;

            Button pauseBtt = new TextButton("||",TextButtonStyle.CreateRaylib());
            mainTable.Add(pauseBtt);

            ScoreLabel_UI = new Label(Score.ToString(),64);
            scoreTable.Add(ScoreLabel_UI);

            ui.Stage.AddElement(mainTable);
            ui.Stage.AddElement(scoreTable);
            ui.SetRenderOrder(10);
        }

        ///If you make mistake, insult urself
        if (Instance != null) throw new Exception("why tf you create more");
        Instance = this;

        /// Finding Existance Entities
        Player = Scene.FindComponents<Ball>().FirstOrDefault();
        planetNextQueue = new Queue<Planet>( );
        
        AssurePlanets();

    }
    public override void OnRemovedFromEntity()
    {
        Instance = null;
    }

    internal void AssurePlanets()
    {
        if(CurrentPlanet == null)
        {
            var radius = RayUtils.RandF(40, 180);
            //If queue have somthing, then pass to current planet
            //else create new entity Planet
            CurrentPlanet = planetNextQueue.TryDequeue(out var planet) ? 
                planet : SampleScene.CreatePlanet(Scene, Vector2.Zero, radius,out _).GetComponent<Planet>();

            Debug.Assert(CurrentPlanet != null);

            //Target the camera control
            Entity.GetComponent<CameraController>().Planet = CurrentPlanet.Entity;

        }
        
        Debug.Assert(CurrentPlanet != null);

        //If there are planet on queue , then 
        if (planetNextQueue.Count < 1)
        {
            AddPlanet();
        }
        

    }

    Vector2 RandomPosition()
    {
        var minLengthRadius = (Scene.ViewPortWidth / 2f) - 30;
        var maxLengthRadius = (Scene.ViewPortWidth / 2f);
        var position = CurrentPlanet != null ? CurrentPlanet.centerTF.Position2 : Vector2.Zero;

        return RayUtils.RandomPointInRing(position, minLengthRadius, maxLengthRadius);
    }

    public Planet AddPlanet()
    {
        var radius = RayUtils.RandF(40, 80);


        SampleScene.CreatePlanet(Scene, RandomPosition(), radius, out Planet planet);

        if(!planetNextQueue.Contains(planet))
            planetNextQueue.Enqueue(planet);

        return planet;
    }
    void ExecuteJumpNext()
    {
        Ball ball = Player;
        Planet next = planetNextQueue.TryDequeue(out Planet? p) ? p : AddPlanet();

        ball.Transform.Parent = next.slotTF;
        ball.Transform.LocalPosition2 = Vector2.Zero;

        if (Entity.TryGetComponent<CameraController>(out var cc))
            cc.Planet = next.Entity;


        var prev = CurrentPlanet;
        CurrentPlanet = next;

        ///Run Fade out animation then Destroy planet
        Core.StartCoroutine(JumpOutAndDestroy(CurrentPlanet, prev));
    }

    /// <summary>
    /// Jump to next <see cref="Planet.slotTF"/>
    /// </summary>
    public void JumpNext()
    {
        ExecuteJumpNext();
        CurrentPlanet.Shake();
        CurrentPlanet.Pulse();

        ScoreLabel_UI.SetFontScale(ScoreLabel_UI.GetStyle().FontScale + 30);
        ScoreLabel_UI.SetText((++Score).ToString());
        AssurePlanets();
        
    }


    IEnumerator JumpOutAndDestroy(Planet curr,Planet prev)
    {
        float duration = 3f;
        float elapse = 0;
        Vector2 from = prev.Transform.Position2;
        Vector2 to = curr.Transform.Position2;
        Vector2 offset = -(to - from) * 2f;

        while (elapse < duration)
        {
            elapse += Time.DeltaTime;
            //Console.WriteLine(elapse);

            var easingX = Easings.EaseSineIn (elapse, from.X, offset.X, duration);
            var easingY = Easings.EaseSineIn(elapse, from.Y, offset.Y, duration);

            prev.Transform.LocalPosition2 = new Vector2(easingX, easingY);
            yield return null;
        }
        Entity.Destroy(prev.Entity);
    }

    internal void CalculateSlotTransform(Ball ball,float circleAmount,Planet curr,Planet next, bool clowise = false)
    {
        var r = next.Radius;
        var offset = next.centerTF.Position2 - curr.centerTF.Position2;

        Debug.Assert(circleAmount <= r*2);

        var x = circleAmount - r;
        var y = (clowise? -1:1) * MathF.Sqrt(r*r - x*x);

        var localSlotPos = new Vector2(x, y);
        var localSlotAngle = RaymathF.Vector2Randian(localSlotPos);

        //Watch for angle error
        var originSlotAngle = RaymathF.Vector2Randian(offset) - MathF.PI/2f;

        var offsetSlotAngle = localSlotAngle + originSlotAngle;
        //Console.WriteLine($"{localSlotAngle * Raylib.RAD2DEG}");

        next.centerTF.SetLocalRotationZ(offsetSlotAngle);
 

    }


    void ICustomInspectorImgui.OnInspectorGUI()
    {
        if(ImGui.Button("Reset Scene"))
        {
            Core.StartTransition(new FadeTransition(() => new SampleScene("Sample", 720, 1280, Color.DARKBLUE, Color.BLACK)));
        }
        ImGui.SliderFloat("ballSpeed",ref BallFloatSpeed,0,1000);

        ImGui.TextUnformatted($"Currnet -> {CurrentPlanet.Entity.Name}");

        if (ImGui.BeginChild("QueueNext"))
        {

            foreach (var p in planetNextQueue)
            {
                ImGui.Text($"{p.Entity.Name}-- {p.Entity.ID}");
            }
            

            ImGui.EndChild();
        }
    }
}
