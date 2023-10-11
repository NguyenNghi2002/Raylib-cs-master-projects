#if true
using Engine;
using Engine.Velcro;
using Raylib_cs;
using System.Numerics;
namespace Golf
{
    public class SlingShotBodyController : Component, IUpdatable
    {
        public event Action<SlingShotBodyController> OnShooted;
        public int UpdateOrder { get; set; }

        public MouseButton Mouse = MouseButton.MOUSE_BUTTON_LEFT;
        private VRigidBody2D body;
        private Vector2 PressedPoint;
        private Vector2 ReleasedPoint;
        private Vector2 shootDirection;

        public override void OnAddedToEntity()
        {
            Entity.TryGetComponent(out body);
        }

        public void Update()
        {
            if (Input.IsMousePressed(Mouse))
            {
                PressedPoint = Input.GetScaledMousePosition();
                shootDirection = Vector2.Zero;
            }

            if (Input.IsMouseDown(Mouse))
            {
                ReleasedPoint = Input.GetScaledMousePosition();

                shootDirection = PressedPoint - ReleasedPoint;
            }

            if (Input.IsMouseReleased(Mouse)
                && Vector2.Distance(PressedPoint, ReleasedPoint) > 10f)
            {
                body?.ApplyLinearImpulse(shootDirection*body.Body.Mass);
                OnShooted?.Invoke(this);
            }
        }
    }



} 
#endif