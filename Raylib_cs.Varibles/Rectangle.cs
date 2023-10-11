    #define Raylib_cs

#if Raylib_cs

using Raylib_cs;
using rRectangle = Raylib_cs.Rectangle;

#endif

using System.Globalization;
using System.Text;

namespace Raysharp
{

    public partial struct Rectangle
    {
        public float x,y,width, height;
        public Rectangle(float x,float y,float width,float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

    }


    public partial struct Rectangle : IEquatable<Rectangle>,IFormattable
    {
        public bool Equals(Rectangle other)
        {
            return x == other.x
                && y == other.y
                && width == other.width
                && height == other.height
                ;
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            StringBuilder sb = new StringBuilder();
            string seperator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;
            sb.Append('<');
            sb.Append(this.x.ToString(format,formatProvider));
            sb.Append(seperator);
            sb.Append(' ');
            sb.Append(this.y.ToString(format,formatProvider));
            sb.Append(seperator);
            sb.Append(' ');
            sb.Append(this.width.ToString(format,formatProvider));
            sb.Append(seperator);
            sb.Append(' ');
            sb.Append(this.height.ToString(format,formatProvider));
            sb.Append('>');
            return sb.ToString();
        }



#if Raylib_cs
        public static implicit operator rRectangle(Rectangle value)
        {
            return new rRectangle(value.x,value.y,value.width,value.height);
        }
#endif 
    }
}