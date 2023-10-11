using Engine;
using Raylib_cs;

namespace RaySharp
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_TRANSPARENT);
            new Core().Run();
        }
    }
}