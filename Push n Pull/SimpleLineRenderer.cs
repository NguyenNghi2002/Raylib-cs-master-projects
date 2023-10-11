using Engine;
using Genbox.VelcroPhysics.Tools.TextureTools;
using Raylib_cs;
using System.Numerics;

internal class SimpleLineRenderer : RenderableComponent
{
    rTexture Texture;
    public Vector2[] Points = new Vector2[2];
    public SimpleLineRenderer()
    {
    }

    public override void Render()
    {
        Rlgl.rlSetLineWidth(2);
        Raylib.DrawLineStrip(Points,Points.Length,Color.WHITE);
    }
}