using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    internal static class Insist
    {
        public static void IsNotNull(object obj,string falseMessage)
        {
            if (obj == null)
                throw new NullReferenceException(falseMessage);

        }
        public static void IsTrue(bool condition,string falseMessage)
        {
            if (!condition)
                throw new (falseMessage);
        }

        internal static void IsFalse(bool condition, string falseMessage)
        {
            if (condition)
                throw new(falseMessage);
        }
    }

    internal static class Time
    {
        public static float TotalTime => (float)Raylib.GetTime();

        public static float UnscaledDeltaTime => Raylib.GetFrameTime();
    }
}
