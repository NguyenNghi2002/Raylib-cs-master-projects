using Raylib_cs;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Raylib.InitWindow(1280,720,"Window");

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();




                Raylib.EndDrawing();
            }
        }
    }
    }
}