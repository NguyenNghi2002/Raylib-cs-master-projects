using Raylib_cs;
using Raysharp;
using System.Diagnostics;
using System.Numerics;

namespace ConsoleApp2
{
    internal class Program
    {

        public static Vector2 LerpCoord(Rectangle src,Rectangle dest,Vector2 sourcePoint)
        {
            var coord = RayUtils.GetCoord01(src,sourcePoint);
            return coord * new Vector2(dest.width, dest.height);
        }

        static void Main(string[] args)
        {
            Raylib.InitWindow(1280,720,"Convert");
            Rectangle rec1 = new Rectangle(30,30,400,200);
            Rectangle rec2 = new Rectangle(500,30,300,600);
            Rectangle recF = new Rectangle(0,0,Raylib.GetScreenWidth(),Raylib.GetScreenHeight());
            while (!Raylib.WindowShouldClose())
            {


                var mousePos1 = LerpCoord(recF, rec1, Raylib.GetMousePosition()) ;

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.BLANK);
                Raylib.DrawRectangleRec(rec1,Color.RED);


                //Raylib.DrawText(coord1.ToString(),0,0,30,Color.PINK);
                //Raylib.DrawText(coord2.ToString(),0,30,30,Color.DARKGREEN);


                Raylib.DrawRectangleRec(rec2,Color.GREEN);

                Raylib.DrawTextEx(Raylib.GetFontDefault(),mousePos1.ToString(),mousePos1 + rec1.TopLeft(),30,3,Color.PINK);
                Raylib.DrawCircleV(mousePos1 + rec1.TopLeft(),2,Color.PINK);
                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }
}