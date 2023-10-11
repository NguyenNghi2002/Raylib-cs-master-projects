using Engine;
using Engine.SceneManager;
using Engine.UI;
using Raylib_cs;
using System.Runtime.ExceptionServices;
using Raylib_cs.Extension;
using System.Numerics;
using System.Timers;
using ImGuiNET;
using Engine.Velcro;
using Genbox.VelcroPhysics.Dynamics;
using Engine.Velcro.Unit;
using Engine.Timer;
using Engine.DefaultComponents.Render.Primitive;

(new OmuApplication()).Run();

public class OmuApplication : Core
{
    public override void Intitialize()
    {
        base.Intitialize();
        Managers.Add(new ImguiEntityManager());
        Scene = new PlayScene();
    }
}

public class PlayScene : Scene
{
    public PlayScene():this("playScene",1280,720,new Color(0,0,20),Color.BLANK){}
    public PlayScene(string sceneName, int width, int height, Color backgroundColor, Color letterBoxClearColor) : base(sceneName, width, height, backgroundColor, letterBoxClearColor)
    {}

    public override void OnLoad()
    {

    }
    public override void OnBegined()
    {
        GetOrCreateSceneComponent<VWorld2D>().World.Gravity = new Microsoft.Xna.Framework.Vector2(0, 0f);

        var cursorAim = CreateEntity("cursor_aim")
            //Physic
            .AddComponent(new VRigidBody2D())
                .SetBodyType(BodyType.Dynamic)
            .AddComponent(new VCollisionCircle(70))

            //Behavior
            .AddComponent(new AimCursor())

            //Sprite
            .AddComponent(new CircleRenderer(10,new Color(100,100,200)))
            .Entity
            ;

        var followAim = CreateEntity("follor_aim")
            //Behavior
            .AddComponent(new FollowAim(cursorAim))
            //Sprite
            .AddComponent(new CircleRenderer(50, new Color(200, 255, 200)))
            .SetRenderOrder(-1)
            ;


        for (int i = 0; i < 1; i++)
        {
            var targetOrb = CreateEntity("target")
                .Move(400 + i * 62, 400)
            //Physic
            .AddComponent(new VRigidBody2D())
                .SetBodyType(BodyType.Kinematic)
            .AddComponent(new VCollisionCircle(30))
                .SetSensor(true)

            //Behavior
            .AddComponent(new DashOrb())
            .AddComponent(new OrbSpawner())
            .AddComponent(new CircleRenderer(30, new Color(200, 55, 20, 255)))
            ;
        }
        
    }
}

public class OrbSpawner : Component, IUpdatable
{
    int IUpdatable.UpdateOrder { get; set; }
    ITimer spawnTImer;
    public override void OnAddedToEntity()
    {
        spawnTImer =  Core.Schedule(4f, true, null, (ctx) =>
        {
            var targetOrb = Scene.CreateEntity("target")
                .MoveTo(Transform.Position2.X + Random.Shared.Next(-100, 100), Transform.Position2.Y + Random.Shared.Next(-100, 100))
            //Physic
            .AddComponent(new VRigidBody2D())
                .SetBodyType(BodyType.Kinematic)
            .AddComponent(new VCollisionCircle(30))
                .SetSensor(true)

            //Behavior
            .AddComponent(new DashOrb())
            .AddComponent(new OrbSpawner())

            .AddComponent(new CircleRenderer(30, new Color(200, 55, 20, 255)))
            ;
        });
    }
    public override void OnRemovedFromEntity()
    {
        spawnTImer.Stop();
    }
    void IUpdatable.Update()
    {
        
    }
}
public class Orb : Component
{
    public AimCursor cursor;
    public Orb() { }
    public override void OnRemovedFromEntity()
    {
        cursor = null;

    }
    public override void OnAddedToEntity()
    {
        var cols = Entity.GetComponent<VCollisionCircle>();


        cols.OnCollision = (a, b, ct) =>
        {
            if(cursor == null)
            {
                cursor = (b.UserData as Entity).GetComponent<AimCursor>();
                cursor.aimedOrbs.Add(this);

            }
        };
        cols.OnSeparation = (a, b, ct) =>
        {
            if (cursor != null)
            {
                Entity.GetComponent<CircleRenderer>().Color = Color.GRAY;
                cursor.aimedOrbs.Remove(this);


                cursor = null;
            }
        };
    }
     public virtual void OnClicked()
    {
        Entity.Destroy(this.Entity);
    }
}
public class DashOrb : Orb
{
    public override void OnClicked()
    {
        cursor.Move(Vector2.UnitY * 200);

        Entity.Destroy(this.Entity);
    }
}
public class FollowAim : Component, IUpdatable
{
    Entity cursorEntity;
    AimCursor cursorAim;
    public int UpdateOrder { get; set; }

    public FollowAim(Entity cursor)
    {
        cursorEntity = cursor;
    }
    public override void OnAddedToEntity()
    {
        cursorAim = cursorEntity.GetComponent<AimCursor>();
    }

    public void Update()
    {
        if(cursorAim.aimedOrbs.Count != 0)
            Transform.Position2 = RaymathF.SmoothDamp(Transform.Position2, cursorAim.aimedOrbs[0].Transform.Position2, 15 * Time.DeltaTime);
        else
            Transform.Position2 = RaymathF.SmoothDamp(Transform.Position2,cursorAim.Transform.Position2,15 * Time.DeltaTime);
    }
}
public class AimCursor : Component,IUpdatable
{
    public List<Orb> aimedOrbs = new();

    private Controller _controller ;
    private Vector2 inputDir;
    float AccelerateSpeed = 7000f;
    float maxSpeed = 400f;
    VRigidBody2D _rb;

    public int UpdateOrder { get; set; }

    public override void OnAddedToEntity()
    {
        _rb = Entity.GetComponent<VRigidBody2D>();

        _controller = new Controller();
        _controller.ControlActions.Add(KeyboardKey.KEY_UP   , () => inputDir -= Vector2.UnitY);
        _controller.ControlActions.Add(KeyboardKey.KEY_DOWN , () => inputDir += Vector2.UnitY);
        _controller.ControlActions.Add(KeyboardKey.KEY_LEFT , () => inputDir -= Vector2.UnitX);
        _controller.ControlActions.Add(KeyboardKey.KEY_RIGHT, () => inputDir += Vector2.UnitX);

    }
    public void Move(Vector2 dir)
    {
        _rb.Body.LinearVelocity += (AccelerateSpeed * dir.ToMVec2() * VConvert.DisplayToSim * Time.DeltaTime);
    }
    public void Update()
    {
        inputDir = Vector2.Zero;
        _controller.UpdateController();
        bool isControlling = inputDir.Length() != 0;

        //normalize inputDirection
        inputDir = isControlling ? inputDir.Normalize() : inputDir;

        _rb.Body.LinearDamping = isControlling ? 0f: 20f ;
        if (isControlling)
        {
            Move(inputDir);
            if (_rb.Body.LinearVelocity.Length() > maxSpeed * VConvert.DisplayToSim)
                _rb.Body.LinearVelocity = _rb.Body.LinearVelocity.ToSVec2().ClampMaxLength(maxSpeed * VConvert.DisplayToSim).ToMVec2();
        }

        if (aimedOrbs.Count > 0)
        {
            Entity.GetComponent<CircleRenderer>().Color = Color.GREEN;
            var keysPressedCount = IsOneOfKeyPressed(KeyboardKey.KEY_Z, KeyboardKey.KEY_X);
            for (int i = 0; i < keysPressedCount && aimedOrbs.Count > 0; i++)
            {
                aimedOrbs[0].OnClicked();
                aimedOrbs.RemoveAt(0);
            }
        }
        else
        {
            this.Entity.GetComponent<CircleRenderer>().Color = Color.BLUE;

        }
    }

    public static int IsOneOfKeyPressed(params KeyboardKey[] keys)
    {
        var keysPresses = Input.CurrentKeysPressed.Where(k => keys.Contains(k));
        
        return keysPresses.Count();
    }
}


public class Controller
{
    public Dictionary<KeyboardKey, Action> ControlActions = new();

    public void UpdateController()
    {
        var requestActions = from a in ControlActions
                             where Input.IsKeyDown(a.Key)
                             select a.Value;
        foreach (var action in requestActions)
                action?.Invoke();
    }

}

