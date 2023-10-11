
using Engine;
using Genbox.VelcroPhysics.Dynamics.Solver;
using Raylib_cs;
using System.Numerics;

public class KinematicMovement : Component, IUpdatable
{
    float speedVelocity = 60;
    float speedDampingOnAir = 0.03f;
    float speedDampingOnGround = 0.45f;

    float JumpHeight = 25f;
    float JumpToApexTime = 0.30f;
    float jumpVelocity;

    float FallAcceleration = 0.7f;

    float jumpBufferTime = 0.5f;
    float jumpBufferElapse;

    float CoyoteTime = 0.1f;
    float coyoteElapse;

    float gravity ;
    Vector2 _rawInput;
    public Vector2 RawInput => _rawInput;
    public Vector2 velocity;

    //KinematicController2D _controller;
    public KinematicController2D controller { get; protected set; }
    public bool ControllerEnabled = true;
    public bool LockInput = false;

    public int UpdateOrder { get; set; }

    public override void OnAddedToEntity()
    {
        controller = Entity.GetComponent<KinematicController2D>();

        gravity = (2*JumpHeight )/ MathF.Pow(JumpToApexTime,2);
        jumpVelocity = gravity * JumpToApexTime;
    }
    void GatherInput( bool horizontal = true, bool verticle = true)
    {
        if (LockInput) return;
        
        if (horizontal)
        {
            if (controller.collisions.Bottom)
                _rawInput.X = 0;

            if (Engine.Input.IsKeyDown(KeyboardKey.KEY_LEFT)) _rawInput -= Vector2.UnitX;
            if (Engine.Input.IsKeyDown(KeyboardKey.KEY_RIGHT)) _rawInput += Vector2.UnitX;
            _rawInput.X =  _rawInput.X.Clamp(-1f, 1f);
            Console.WriteLine(RawInput);
        }
        if (verticle)
        {
            _rawInput.Y = 0;

            if (Engine.Input.IsKeyDown(KeyboardKey.KEY_UP)) _rawInput -= Vector2.UnitY;
            if (Engine.Input.IsKeyDown(KeyboardKey.KEY_DOWN)) _rawInput += Vector2.UnitY;
        }
    }
    public void Update()
    {
        var jumpHeld = Engine.Input.IsKeyDown(KeyboardKey.KEY_SPACE);
        var jumpPressed = Engine.Input.IsKeyPressed(KeyboardKey.KEY_SPACE);
        var isGrounded = controller.collisions.Bottom;
        var isTouchingWall = (controller.collisions.Right || controller.collisions.Left);


        //Prevent saved velocity accelerating from gravity
        velocity.Y = (controller.collisions.Bottom || controller.collisions.Top) ? 0 : velocity.Y;
        velocity.X = (isTouchingWall) ? 0 : velocity.X;

        
        if(ControllerEnabled) GatherInput();


         UpdateMovement(_rawInput,speedVelocity);
        

        //Coyote 
        if (isGrounded)
        {
            coyoteElapse = CoyoteTime;
        }
        else if(!isGrounded && (velocity.Y > 0 || (!jumpHeld && velocity.Y <= 0)))
        {
           if (coyoteElapse > 0) coyoteElapse -= Time.DeltaTime;
        }

        // Jump buffer
        if (jumpPressed )
        {
            jumpBufferElapse = jumpBufferTime;
        }
        if (jumpHeld )
        {
            jumpBufferElapse -= Time.DeltaTime;
            jumpBufferElapse = MathF.Max(0,jumpBufferElapse);
        }


        //Execute Jump buffer 
        if(jumpBufferElapse > 0 && coyoteElapse > 0  && jumpHeld || (jumpPressed & isGrounded))
        {
            velocity.Y = -jumpVelocity;
            jumpBufferElapse = 0;
            coyoteElapse = 0;
        }

        /// Cancle jump
        if (Engine.Input.IsKeyReleased(KeyboardKey.KEY_SPACE)  && !isGrounded)
        {
            if(velocity.Y < 0) velocity.Y *= 0.8f;
            coyoteElapse = 0;
        }

        //Gravity
        velocity.Y += gravity * Time.DeltaTime;


        //Apply FinalMovement
        controller.Move(velocity * Time.DeltaTime);
    }

    public override void OnDebugRender()
    {
        Raylib.DrawLineEx(Transform.Position2, Transform.Position2 + velocity /4f , 2, Color.GRAY);
        Raylib.DrawText(jumpBufferElapse.ToString(),50,50,40,Color.GREEN);
        Raylib.DrawText(coyoteElapse.ToString(),50,90,40,Color.GREEN);
    }

    protected void UpdateMovement(Vector2 direction,float maxSpeed)
    {
        velocity.X = controller.collisions.Bottom ?
            RaymathF.SmoothDamp(velocity.X, direction.X * maxSpeed, speedDampingOnGround * Time.TimeScale) :/* Grounding */
            RaymathF.SmoothDamp(velocity.X, direction.X * maxSpeed, speedDampingOnAir * Time.TimeScale ); /* Airbore */
    }
}
