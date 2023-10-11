using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raylib_cs.UI.Extra
{
    internal static class ColorExt
    {
        public static Color Create(Color color, int alpha) => new Color(color.r, color.g, color.b, alpha);
        internal static bool HasValue(this Color color)
            => color.r != 0
            && color.g != 0
            && color.b != 0
            && color.a != 0;

    }
}
