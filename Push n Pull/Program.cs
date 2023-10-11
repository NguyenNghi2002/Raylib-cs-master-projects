using Engine;
using Engine.Renderering;
using Genbox.VelcroPhysics.Collision;
using Genbox.VelcroPhysics.Utilities;
using Raylib_cs;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;


new GameApp().Run();

public class  GameApp : Engine.Core
{
    public override void Intitialize()
    {
        
        Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
        //a.format = PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8;
        //Raylib.ImageFormat(ref a , PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8);
        base.Intitialize();
        //Debugging.EnableDrawDebug = true;
        Raylib.SetTargetFPS(60);


        Scene = new GameplayScene();


        Managers.Add(new ImguiEntityManager());
    }
}


