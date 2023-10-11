using Raylib_cs;
using System.Numerics;
using System.Xml.Linq;

namespace Raylib.Texturepacker
{
    public class Sprite
    {
        public TextureAtlas Atlas;

        /// <summary>
        /// Sprite name ID
        /// </summary>
        public string Name;
        public int OriginX,OriginY;
        public int PositionX,PositionY;
        public int SourceWidth,SourceHeight;
        public int Padding;
        public bool IsTrimmed;
        public int TrimmedX,TrimmedY;
        public int TrimmedWidth,TrimmedHeight;

        public Vector2 Position => new Vector2(PositionX, PositionY);
        public Rectangle Source => IsTrimmed ?
            new Rectangle(TrimmedX,TrimmedY,TrimmedWidth,TrimmedHeight):
            new Rectangle(PositionX,PositionY,SourceWidth,SourceHeight);

        public Sprite(Texture2D texture)
        {
            SourceWidth = texture.width;
            SourceHeight = texture.height;
        }

        public Sprite(TextureAtlas textureAtlas,XElement xElement)
        {
            Atlas = textureAtlas;
            Name = (string?)xElement.Attribute("nameId") ;
            OriginX = (int?)xElement.Attribute("originX") ?? 0;
            OriginY = (int?)xElement.Attribute("originY") ?? 0;
            PositionX = (int?)xElement.Attribute("positionX") ?? 0;
            PositionY = (int?)xElement.Attribute("positionY") ?? 0;
            SourceWidth = (int?)xElement.Attribute("sourceWidth") ?? 0;
            SourceHeight = (int?)xElement.Attribute("sourceHeight") ?? 0;
            IsTrimmed = (bool?)xElement.Attribute("isTrimmed") ?? false;
            TrimmedX = (int?)xElement.Attribute("trimRecX") ?? 0;
            TrimmedY = (int?)xElement.Attribute("trimRecX") ?? 0;
            TrimmedWidth = (int?)xElement.Attribute("trimRecWidth") ?? 0;
            TrimmedHeight = (int?)xElement.Attribute("trimRecHeight") ?? 0;

        }
    }
}