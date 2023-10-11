using Engine;
using Engine.DefaultComponents.Render.Primitive;
using Engine.Timer;
using Microsoft.Win32.SafeHandles;
using Raylib_cs;
using System.Collections;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Permissions;

public class Ball : Component, IUpdatable
{
    int IUpdatable.UpdateOrder { get; set; }

    public override void OnAddedToEntity()
    {
        var planet = GameSceneManager.Instance.CurrentPlanet;

        

        var centerTF = Scene.FindEntityInParent(planet.Entity, e => e.Name.Contains("center")).Transform;
        var slotTF = Scene.FindEntityInParent(centerTF.Entity, e => e.Name.Contains("slot")).Transform;

        ///If batman
        if (Transform.Parent == null)
        {
            Transform.SetParent(slotTF,false);
        }
    }
    float TweenTimer = 0;
    float TweenTime = 1f;

    KeyboardKey controlKey = KeyboardKey.KEY_SPACE;
    void IUpdatable.Update()
    {
        
        var playerball = GameSceneManager.Instance.Player;
        var curr = GameSceneManager.Instance.CurrentPlanet;
        var nextQueue = GameSceneManager.Instance.planetNextQueue;
        if (playerball != this || curr == null)
            return;

        if (Transform.Parent != null && ballSpining == null)
        {
            var originEn = Transform;

            if (Input.IsKeyPressed(controlKey))
            {
                ballReturning?.Stop();
                ballReturning = null;
                originEn.LocalPosition = Vector3.Zero;
            }
            if (Input.IsKeyDown(controlKey))
            {
                originEn.LocalPosition2 += Vector2.UnitY * Time.DeltaTime * GameSceneManager.Instance.BallFloatSpeed;
            }
            if (Input.IsKeyReleased(controlKey) && nextQueue.TryPeek(out var next) )
            {

                var ballDis = Vector2.Distance(curr.centerTF.Position2, this.Transform.Position2);
                var nextPlanetDis = Vector2.Distance(curr.centerTF.Position2, next.centerTF.Position2);
                if (ballDis >= nextPlanetDis - next.Radius && ballDis <= nextPlanetDis + next.Radius)
                {
                    ///READY TO JUMP 
                    ///

                    //Console.WriteLine("Succes");
                    var ballDisRelateToMinSurface = (ballDis - (nextPlanetDis - next.Radius) ) ;

                    bool clowise = Convert.ToBoolean(Random.Shared.Next(0, 2));
                    Console.WriteLine(Random.Shared.Next(0, 2));
                    GameSceneManager.Instance.CalculateSlotTransform(this,ballDisRelateToMinSurface,curr,next,clowise);

                    var LocaltoSlot = next.slotTF.Position2 - curr.centerTF.Position2;
                    var angleToSlot = MathF.Atan2(LocaltoSlot.Y, LocaltoSlot.X);
                    startA = Transform.EulerRotation.Z * Raylib.RAD2DEG ;
                    endA = angleToSlot * Raylib.RAD2DEG;


                    ///Run animation
                    ballSpining =  Core.StartCoroutine(RotateTweeningBall(curr.centerTF.EulerRotation.Z , angleToSlot -MathF.PI/2f,clowise));


                    //Console.WriteLine($"{ballDisRelateToMinSurface}/{next.Radius}");
                }
                else
                {
                    ///FAIL TO JUMP 
                    //Console.WriteLine("fail");

                    ballReturning =  Core.StartCoroutine(TweenLocal(Transform.LocalPosition2 , Vector2.Zero, (e,f,o,d)=> Easings.EaseBounceOut(e,f,o,d) ));;
                }
            }
        }

        
    }

    float startA, endA;
    ICoroutine? ballReturning,ballSpining;
    public override void OnDebugRender()
    {
        var c = GameSceneManager.Instance.CurrentPlanet;
        Raylib.DrawRing(c.centerTF.Position2,c.Radius - 5,c.Radius,startA ,endA ,100,Color.RED);
    }

    IEnumerator RotateTweeningBall(float start,float end,bool clockWise )
    {      
        var pi2 = 2 * MathF.PI;
        //start = (start % pi2 + pi2) % pi2;
        //end = (end % pi2 + pi2) % pi2;
        float speed = 4;
        var offset = end - start;

        if(clockWise) offset = offset >= 0 ? offset : (offset + pi2) % pi2;
        else offset = offset < 0 ? offset : (offset - pi2) % pi2;


        float duration = MathF.Abs(offset)/speed;
        float elapse = 0;
        while (elapse < duration)
        {
            elapse += Time.DeltaTime;
            //Console.WriteLine(elapse);

            var easingValue = Easings.EaseSineIn(elapse, start, offset, duration);
            GameSceneManager.Instance.CurrentPlanet.centerTF.SetEulerRotation(new (0,0,easingValue));
            yield return null;
        }
        //Console.WriteLine($"{start * Raylib.RAD2DEG} >>> {end * Raylib.RAD2DEG}");
        //Console.WriteLine($"{offset * Raylib.RAD2DEG}");
        
        
        GameSceneManager.Instance.JumpNext();
        ballSpining = null;
    }
    IEnumerator TweenLocal( Vector2 from,Vector2 to,Func<float,float,float,float,float> easingAction)
    {
        var offset = to-from;
        float duration = .43f;
        float elapse = 0;

        while (elapse < duration)
        {
            elapse += Time.DeltaTime;
            Console.WriteLine(elapse);


            var easingX = easingAction.Invoke(elapse,from.X,offset.X,duration);
            var easingY = easingAction.Invoke(elapse,from.Y,offset.Y,duration);
            

              
            Transform.LocalPosition2 = new Vector2(easingX,easingY);
            yield return null;
        }

        Transform.LocalPosition2 = to;
        ballReturning = null;
    }

}
   