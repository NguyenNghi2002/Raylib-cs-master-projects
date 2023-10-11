using Raylib_cs;
using System.Numerics;
using Engine.UI;


namespace Engine.UI
{
    public class TextureDrawable : IDrawable
    {
        public Texture2D? Texture;
        public float LeftWidth { get; set; }
        public float RightWidth { get; set; }
        public float TopHeight { get; set; }
        public float BottomHeight { get; set; }
        public float MinWidth { get; set; }
        public float MinHeight { get; set; }

        public TextureDrawable(float minWidth,float minHeight,Texture2D? texture)
        {
            this.MinWidth = minWidth;
            this.MinHeight = minHeight;
            this.Texture = texture;
        }
        public TextureDrawable(Texture2D texture) : this(texture.width,texture.height,texture) { }
        public void Draw(float x, float y, float width, float height, Color color)
        {
            if(Texture.HasValue)
            Raylib.DrawTexturePro(Texture.Value, new Rectangle(0, 0, Texture.Value.width, Texture.Value.height), new Rectangle(x, y, width, height), Vector2.Zero, 0, color);
        }

        public void SetPadding(float top, float bottom, float left, float right)
        {
            LeftWidth = left;
            RightWidth = right;
            TopHeight = top;
            BottomHeight = bottom;
        }
    }
}