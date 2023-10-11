using MVec2 = Microsoft.Xna.Framework.Vector2;

using Engine;
using Engine.Velcro;
using Genbox.VelcroPhysics.Extensions.Controllers.ControllerBase;
using Genbox.VelcroPhysics.Extensions.Controllers.Velocity;
using Raylib_cs;
using System.Numerics;
using Engine.Timer;
using System.Net.Sockets;
using Engine.SceneManager;
using Engine.DefaultComponents.Render.Primitive;
using System.Diagnostics;
using Engine.Velcro.Unit;
public class Player : KinematicMovement, IUpdatable
{
    public interface IAbility
    {
        public void BeginExecute(Entity ctx);
        public void Update(Entity ctx);
        public void EndExecute(Entity ctx);
    }

    #region Fields
    SpriteRenderer spriteRenderer;
    Entity beamHit;

    VWorld2D world;
    public int test = 2;
    public int lastDirectionX { get; private set; } = 1;

    public KeyboardKey a;
    float beamLength = int.MaxValue;
    float beamStrength = 50;
    VWorld2D.HitInfo hit; 
    #endregion

    public Dictionary<KeyboardKey[], Player.IAbility> abilitiesSets;

    public override void OnAddedToEntity()
    {
        base.OnAddedToEntity();
        world = Scene.GetOrCreateSceneComponent<VWorld2D>();
        spriteRenderer = Entity.GetComponent<SpriteRenderer>();
        abilitiesSets = new()
        {
            [new KeyboardKey[] {KeyboardKey.KEY_B}] = new SlowdownTime(),
            [new KeyboardKey[] {KeyboardKey.KEY_B}] = new Aiming("aim"),
        };
    }

    void IUpdatable.Update()
    {
        base.Update();

        if (Engine.Input.IsKeyPressed(KeyboardKey.KEY_LEFT))
        {
            spriteRenderer.FlipX = true;
            lastDirectionX = -1;
        }

        if (Engine.Input.IsKeyPressed(KeyboardKey.KEY_RIGHT))
        {
            spriteRenderer.FlipX = false;
            lastDirectionX = 1;
        }

        foreach (var p in abilitiesSets)
        {
            if (p.Key.Any((k=> Engine.Input.IsKeyPressed(k))))
            {
                p.Value.BeginExecute(this.Entity);
            }

            if (p.Key.Any((k => Engine.Input.IsKeyReleased(k))))
            {
                p.Value.EndExecute(this.Entity);
            }

            p.Value.Update(this.Entity);
        }
    }

    public override void OnDebugRender()
    {
    
    }
   

}

