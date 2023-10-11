
using Engine;
using ImGuiNET;
using Raylib_cs;
using System.Numerics;

public class SlowdownTime : Player.IAbility,ICustomInspectorImgui
{
    public float SlowScale = 0.059f;
    public float Startdamping = 0.1f;
    public float Enddamping = 0.7f;

    float  _toAcc = Time.TimeScale;
    float _dampingAcc = 0.1f;
    void Player.IAbility.BeginExecute(Entity ctx)
    {
        _toAcc = SlowScale;
        Console.WriteLine(SlowScale);
        _dampingAcc = Startdamping;
    }

    void Player.IAbility.Update(Entity ctx)
    {
        Time.TimeScale = RaymathF.SmoothDamp(Time.TimeScale, _toAcc, _dampingAcc); 
    }

    void Player.IAbility.EndExecute(Entity ctx)
    {
        _toAcc = 1f;
        _dampingAcc = Enddamping;
    }

    public void OnInspectorGUI()
    {
        ImGui.Text(SlowScale.ToString());
    }
}

public class Aiming : Player.IAbility
{
    string _childName;
    Entity? _aimingEntity;

    bool _active = false;
    float aimmingSpeed = 400;
    public Aiming(string childName)
    {
        _childName = childName;
    }
    void Player.IAbility.BeginExecute(Entity ctx)
    {
        _active = true;
        if(_aimingEntity == null)
        {
            Console.WriteLine("Aimming");
            var c = ctx.Transform.Childs.Find(e => e.Entity.Name.Contains(_childName));
            if(c!= null)
            {
                _aimingEntity = c.Entity;
                _aimingEntity.Transform.Childs.First().Entity.Enable = true;
            }
        }
        else
        {
            _aimingEntity.Transform.Childs.First().Entity.Enable = true;
        }
    }

    void Player.IAbility.EndExecute(Entity ctx)
    {
        _active = false;
        _aimingEntity.Transform.Childs.First().Entity.Enable = false;
        _aimingEntity?.Transform.SetLocalPosition(System.Numerics.Vector2.Zero);
    }

    void Player.IAbility.Update(Entity ctx)
    {
        Vector2 input = Vector2.Zero;
        if (Engine.Input.IsKeyDown(KeyboardKey.KEY_LEFT)) input -= Vector2.UnitX;
        if (Engine.Input.IsKeyDown(KeyboardKey.KEY_RIGHT)) input += Vector2.UnitX;
        if (Engine.Input.IsKeyDown(KeyboardKey.KEY_UP)) input -= Vector2.UnitY;
        if (Engine.Input.IsKeyDown(KeyboardKey.KEY_DOWN)) input += Vector2.UnitY;

        if(_aimingEntity != null && _active)
        {
            _aimingEntity.Transform.LocalPosition2 += input * aimmingSpeed *Time.UnscaledDeltaTime;
            _aimingEntity.Transform.LocalPosition2 = _aimingEntity.Transform.LocalPosition2.ClampMaxLength(60);
        }
    }
}
