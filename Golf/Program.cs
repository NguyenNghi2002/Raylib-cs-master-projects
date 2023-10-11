using Engine;
using Raylib_cs;
using System.Numerics;

using Engine.Velcro.Unit;
using Genbox.VelcroPhysics.Tools.Triangulation.TriangulationBase;
using Genbox.VelcroPhysics.Factories;
using Genbox.VelcroPhysics.Tools.Cutting.Simple;
using Genbox.VelcroPhysics.Tools.Cutting;
using Engine.Renderering;
using MVec3 = Microsoft.Xna.Framework.Vector3;
using MVec2 = Microsoft.Xna.Framework.Vector2;
using Raylib_cs.Extension;

namespace Golf
{
    using MVec2 = Microsoft.Xna.Framework.Vector2;

    internal class Program
    {
        static void Main(string[] args)
        {
            new GolfGame().Run();
        }
    }
    public class GolfGame : Core
    {
        public override void Intitialize()
        { 
            base.Intitialize();
            Scene = new GameplayScene();
        }
    }

#if true


    public class BoxRenderer : RenderableComponent
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public Color Color { get; set; }
        public BoxRenderer(double width, double height, Color? color) : this((float)width, (float)height, color) { }
        public BoxRenderer(float width,float height,Color? color )
        {
            this.Width = width;
            this.Height = height;
            this.Color = color ?? Color.WHITE;
        }
        public override void Render()
        {
            var pos = Transform.Position.ToVec2();
            var scale = Transform.Scale.ToVec2() * new Vector2(Width,Height);
            var rot = Transform.EulerRotation.Z * Raylib.RAD2DEG;
            Raylib.DrawRectanglePro(RectangleExt.CreateRectangle(pos,scale),scale/2f,rot,Color);
        }
    }


#endif

}