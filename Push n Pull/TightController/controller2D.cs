using Engine;
using Engine.Velcro;
using Engine.Velcro.Unit;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Shared;
using Genbox.VelcroPhysics.Utilities;
using Raylib_cs;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using System.Xml.Schema;
using static Engine.Velcro.VWorld2D;

/// <summary>
/// Require <see cref="VCollisionShape"/>
/// </summary>
public class KinematicController2D : Component
{
    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft,bottomRight;
    }
    public struct CollisionInfo
    {
        public bool Top, Bottom, Left, Right;
        public bool ascendSlope;
        public float slopeAngleOld;
        public float slopeAngle;
        internal bool descendSlope;

        public void Reset()
        {
            Top = Bottom = Left = Right = false;
            ascendSlope = descendSlope =  false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }

    const float skinWidth = .015f;

    public int horizontalRayCount = 4;
    public int verticleRayCount = 4;

    public float maxAscendSlope = 60;
    public float maxDescendSlope = 90;

    float horizontalRaySpacing;
    float verticleRaySpacing;

    VCollisionBox collider;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions = new();
    VWorld2D World => Scene.GetOrCreateSceneComponent<VWorld2D>();


    public override void OnAddedToEntity()
    {
        collider = Entity.GetComponent<VCollisionBox>();
        calculateRaySpacing();
        UpdateRayCastOrigins();
    }
    public override void OnRemovedFromEntity()
    {
        collider = null;
    }
    public override void OnDebugRender()
    {
        DrawVerticleRays();
        DrawHorizontalRays();
#if false
        Raylib.DrawText(String.Format($"Bottom {collisions.Bottom}"), 0, 0, 20, Color.YELLOW);
        Raylib.DrawText(String.Format($"Top {collisions.Top}"), 0, 20, 20, Color.YELLOW);
        Raylib.DrawText(String.Format($"Left {collisions.Left}"), 0, 40, 20, Color.YELLOW);
        Raylib.DrawText(String.Format($"Right {collisions.Right}"), 0, 60, 20, Color.YELLOW);
        Raylib.DrawText(String.Format($" {collisions.ascendSlope}"), 0, 80, 20, Color.YELLOW);

        Raylib.DrawText(String.Format($" {collisions.slopeAngle}"), 0, 100, 20, Color.YELLOW);
        Raylib.DrawText(String.Format($" {collisions.slopeAngleOld}"), 0, 120, 20, Color.YELLOW); 
#endif
    }

    #region Interface
    public void Move(Vector2 velocity)
    {
        v = velocity;
        collisions.Reset();
        UpdateRayCastOrigins();

        if (velocity.Y > 0) DescendSlope(ref velocity);
        if (velocity.X != 0) HorizontalCollision(ref velocity);
        if (velocity.Y != 0) VerticleCollision(ref velocity);

        Transform.Position2 += velocity;
        finalV = velocity;
    }

    #endregion


    void DrawHorizontalRays()
    {
        float directionX = MathF.Sign(v.X);
        //float raylength = MathF.Abs(v.X) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrgin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrgin -= Vector2.UnitY * (horizontalRaySpacing * i);

            var a = rayOrgin;
            var b = a + Vector2.UnitX * directionX * 20;
            Raylib.DrawLineV(a, b, i == 0 ? Color.RED : Color.DARKBROWN);
        }
        Raylib.DrawLineEx(Transform.Position2, Transform.Position2 + (finalV / 4f) / Time.DeltaTime, 2, Color.YELLOW);
    }

    void DrawVerticleRays()
    {
        //flip since graphic axis y is fliped
        float directionY = MathF.Sign(v.Y);
        //float raylength = MathF.Abs(v.Y) + skinWidth;

        for (int i = 0; i < verticleRayCount; i++)
        {
            Vector2 rayOrgin = (directionY == 1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrgin += Vector2.UnitX * (verticleRaySpacing * i + v.X);

            var a = rayOrgin;
            var b = a + Vector2.UnitY * directionY * 20;
            Raylib.DrawLineV(a, b, Color.RED);
        }
    }

    void VerticleCollision(ref Vector2 velocity)
    {
        //flip since graphic axis y is fliped
        float directionY =  MathF.Sign(velocity.Y);
        float directionX =  MathF.Sign(velocity.X);
        float rayLength = MathF.Abs(velocity.Y) + skinWidth;
        for (int i = verticleRayCount - 1; i >= 0; i--)
        {

            Vector2 rayOrgin = (directionY == 1) ?  raycastOrigins.bottomLeft : raycastOrigins.topLeft;

            rayOrgin += Vector2.UnitX * (verticleRaySpacing * i + velocity.X );
            
            bool isHitted = World.RayCast((hit) => hit.fixture.IsSensor ? -1f : hit.fraction,rayOrgin, Vector2.UnitY * directionY,rayLength,out var hit);;
            if (isHitted && !hit.fixture.IsSensor)
            {
                velocity.Y = (hit.distance - skinWidth ) * directionY;
                rayLength = hit.distance;

                if (collisions.ascendSlope)
                {
                    velocity.X = MathF.Tan(collisions.slopeAngle * Raylib.DEG2RAD) * Math.Abs(velocity.Y);
                }

                collisions.Top = directionY == -1;
                collisions.Bottom = directionY == 1;
            }
        }

        if (collisions.ascendSlope)
        {
            rayLength = MathF.Abs(velocity.X) + skinWidth;
            Vector2 rayOrigin = ((directionX == 1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft) 
                + (Vector2.UnitY * velocity.Y);
            bool hitted = World.RayCast((hit) => hit.fixture.IsSensor ? -1f : hit.fraction,rayOrigin, Vector2.UnitX * directionX, rayLength,out var hit);

            if (hitted && !hit.fixture.IsSensor)
            {
                float slopeAngle = Angle(-Vector2.UnitY, hit.normal) * Raylib.RAD2DEG ;
                if (slopeAngle != collisions.slopeAngle)
                {
                    velocity.X = (hit.distance - skinWidth ) * directionX ;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }
    void HorizontalCollision(ref Vector2 velocity)
    {
        //flip since graphic axis y is fliped
        float directionX = MathF.Sign(velocity.X);
        float rayLength = MathF.Abs(velocity.X) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrgin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrgin -= Vector2.UnitY * (horizontalRaySpacing * i ) ;

            var hitted =  World.RayCast((hinfo)=> hinfo.fixture.IsSensor ? -1 : hinfo.fraction, rayOrgin, Vector2.UnitX * directionX, rayLength,out var hit);
            if (hitted && !hit.fixture.IsSensor )
            {
                float slopeAngle = Angle(-Vector2.UnitY,hit.normal) * Raylib.RAD2DEG;
                if ( slopeAngle <= maxAscendSlope)
                {
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.X -= distanceToSlopeStart * directionX;
                    }
                    AscendSlope(ref velocity, slopeAngle * Raylib.DEG2RAD);
                    velocity.X += distanceToSlopeStart * directionX;
                }
                
                if (!collisions.ascendSlope || slopeAngle > maxAscendSlope)
                {
                    velocity.X = (hit.distance - skinWidth ) * directionX;
                    rayLength =  MathF.Min(hit.distance, rayLength);

                    if (collisions.ascendSlope)
                    {
                         velocity.Y = -MathF.Tan(collisions.slopeAngle * Raylib.DEG2RAD) * Math.Abs(velocity.X);
                    }

                    collisions.Left = directionX == -1;
                    collisions.Right = directionX == 1;

                }

            }
        }

    }

    void DescendSlope(ref Vector2 velocity)
    {
        float directionX = MathF.Sign(velocity.X);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        bool isHitted = World.RayCast((hinfo) => hinfo.fixture.IsSensor ? -1 : hinfo.fraction, rayOrigin, Vector2.UnitY, int.MaxValue, out var hit);
        if (isHitted && !hit.fixture.IsSensor)
        {
            float slopeAngle = Angle(hit.normal,-Vector2.UnitY) ;
            if(slopeAngle != 0 && slopeAngle * Raylib.RAD2DEG <= maxDescendSlope)
                ///Check if moving shame hit distance
                if(MathF.Sign(hit.normal.X) == directionX)
                    ///Check if velocity.Y is larger than rayhit distance
                    if(hit.distance - skinWidth <=MathF.Tan(slopeAngle * Raylib.DEG2RAD) * MathF.Abs(velocity.X))
                    {
                        float moveDistance = MathF.Abs(velocity.X);
                        float descendVelocityY = MathF.Sin(slopeAngle) * moveDistance;
                        velocity.X = MathF.Cos(slopeAngle) * moveDistance * MathF.Sign(velocity.X);
                        velocity.Y += descendVelocityY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendSlope = true;
                        collisions.Bottom = true;
                    }
        }
    }

    void AscendSlope(ref Vector2 velocity, float angleInRad)
    {
        //Console.WriteLine( velocity);
        var moveDistance = MathF.Abs(velocity.X);
#if true
        var climbVelocityY = -moveDistance * MathF.Sin(angleInRad);
        if (velocity.Y >= climbVelocityY)
        {
            velocity.Y = climbVelocityY ;
            velocity.X = moveDistance * MathF.Cos(angleInRad) * MathF.Sign(velocity.X);

            collisions.Bottom = true;
            collisions.ascendSlope = true;
            collisions.slopeAngle = angleInRad * Raylib.RAD2DEG;
        } 
#else
        velocity.Y = -moveDistance * MathF.Sin(angleInRad);
        velocity.X = moveDistance * MathF.Cos(angleInRad) * MathF.Sign(velocity.X);

#endif
        //Console.WriteLine((angleInRad * Raylib.RAD2DEG) + "--" + velocity);
    }

    Vector2 v = Vector2.Zero;
    Vector2 finalV = Vector2.Zero;
    void UpdateRayCastOrigins()
    {
        ///Shrink double skinWidth
        float hWidth = collider.Width / 2f - skinWidth * 2;
        float hHeight = collider.Height / 2f - skinWidth * 2;
        Vector2 offset = Transform.Position2;

        ///Calculate Origins
        raycastOrigins.bottomLeft = new Vector2(-hWidth,hHeight) + offset; 
        raycastOrigins.bottomRight = new Vector2(hWidth,hHeight) + offset; 
        raycastOrigins.topLeft = new Vector2(-hWidth,-hHeight) + offset; 
        raycastOrigins.topRight = new Vector2(hWidth,-hHeight) + offset; 
    }
    /// <summary>
    /// Calculate gaps between space
    /// </summary>
    void calculateRaySpacing()
    {
        /// If raycount = 2, the gap should be full dimension
        horizontalRayCount = Math.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticleRayCount = Math.Clamp(verticleRayCount, 2, int.MaxValue);

        ///Reference from 
        float width = (collider.Width / 2f - skinWidth * 2) * 2;
        float height = (collider.Height / 2f - skinWidth * 2) * 2;
        horizontalRaySpacing = height / (horizontalRayCount - 1);
        verticleRaySpacing = width / (verticleRayCount - 1);
    }
    float Angle(Vector2 a, Vector2 b)
    {
        float dot = Vector2.Dot(a, b);
        float length = a.Length() * b.Length();
        return MathF.Acos(dot / length);
    }
    
}