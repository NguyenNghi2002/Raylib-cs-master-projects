using OpenGL;
using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;
using static Raylib_cs.Rlgl;

namespace a
{
    public static class Program
    {
        public static Camera3D camera3D;
        public unsafe static void Main(string[] args)
        {
            var a = Raylib.GetMouseWheelMoveV();
            

            Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_TRANSPARENT);
            //Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
            Raylib.InitWindow(1280, 720, "layoutTable");

            var tile = GenImageChecked(1280,720,30,30,Color.RED,Color.GREEN);
            var cellImg = GenImageCellular(500, 500, 30);
            var noiseImg = GenImageWhiteNoise(500, 500, 0.2f);
            var noiseTxt = LoadTextureFromImage(tile);

            Model cube = Raylib.LoadModelFromMesh(GenMeshCube(1,1,1)) ;



            Camera2D camera2D = new Camera2D(Vector2.Zero, Vector2.Zero, 0, 1f);
            camera3D = new Camera3D(new Vector3(10, 10, 10), Vector3.Zero, Vector3.UnitY, 45f, CameraProjection.CAMERA_PERSPECTIVE);
            RenderTexture2D blurRender =   LoadRenderTexture(1280,720);
            RenderTexture2D mainRender =   LoadRenderTexture(1280,720);


            var hexModel =  Raylib.LoadModelFromMesh(Raylib.GenMeshKnot(2,2,45,5));

            Console.WriteLine("0-------------------");
            Shader blurShader = LoadShader(null,"blur.glsl");
            bool horizontal = false;
            Raylib.SetTraceLogLevel(TraceLogLevel.LOG_WARNING);
            SetCameraMode(camera3D,CameraMode.CAMERA_THIRD_PERSON);
            SetCameraMoveControls(KeyboardKey.KEY_W,KeyboardKey.KEY_S,KeyboardKey.KEY_D,KeyboardKey.KEY_A,KeyboardKey.KEY_SPACE,KeyboardKey.KEY_LEFT_ALT);
            Raylib.SetTextureFilter(blurRender.texture,TextureFilter.TEXTURE_FILTER_BILINEAR);
            Raylib.SetTextureWrap(blurRender.texture,TextureWrap.TEXTURE_WRAP_CLAMP);


            RenderTexture2D[] pingpongFBO =
            {
                LoadRenderTexture(1280, 720),
                LoadRenderTexture(1280, 720),
            };
            Raylib.SetTextureWrap(pingpongFBO[0].texture,TextureWrap.TEXTURE_WRAP_CLAMP);
            Raylib.SetTextureWrap(pingpongFBO[1].texture,TextureWrap.TEXTURE_WRAP_CLAMP);

            mainRender.texture.format = PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8;


            while (!Raylib.WindowShouldClose())
            {
                UpdateCamera(ref camera3D);
                

                if (Raylib.IsKeyPressed(KeyboardKey.KEY_X))
                {
                    rlLoadFramebuffer(200, 200);

                    horizontal = !horizontal;
                    SetShaderValue(blurShader,GetShaderLocation(blurShader,"horizontal"),horizontal,ShaderUniformDataType.SHADER_UNIFORM_INT);
                }

#if true
                Raylib.BeginTextureMode(mainRender);
                Raylib.BeginMode3D(camera3D);
                Raylib.ClearBackground(Color.BLACK);

                Raylib.DrawGrid(10, 10);
                DrawSphere();
                DrawCube();
                Raylib.DrawModel(hexModel,Vector3.One * 2,2,Color.WHITE); ;

                Raylib.EndMode3D();
                Raylib.EndTextureMode(); 
#endif
                //*********************************************************************8
                Raylib.BeginTextureMode(blurRender);
                Raylib.BeginMode3D(camera3D);
                Raylib.ClearBackground(Color.BLACK);

                DrawSphere();
                DrawCube();

                Raylib.EndMode3D();
                Raylib.EndTextureMode();

            //*********************************************************************8

               

                var src = new Rectangle(0, 0, blurRender.texture.width, -blurRender.texture.height);
                var dest = new Rectangle(0, 0, blurRender.texture.width, blurRender.texture.height);



                Raylib.BeginShaderMode(blurShader);
                int finalBuffer = 0;
                for (int i = 0; i < 10; i++)
                {
                    Raylib.BeginTextureMode(pingpongFBO[i%2] );
                    Raylib.SetShaderValue(blurShader, GetShaderLocation(blurShader, "horizontal"), i%2, ShaderUniformDataType.SHADER_UNIFORM_INT);

                    if (i == 0)
                    {

                        Raylib.DrawTexturePro(blurRender.texture, src, dest, Vector2.Zero, 0, Color.WHITE);
                    }

                    else 
                        Raylib.DrawTexturePro(pingpongFBO[(i+1)%2].texture, src, dest, Vector2.Zero, 0, Color.WHITE); 

                    Raylib.EndTextureMode();
                    finalBuffer = i%2;
                }
                Raylib.EndShaderMode();


                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.BLANK);


                //DrawScene();


                mainRender.texture.format = PixelFormat.PIXELFORMAT_COMPRESSED_PVRT_RGBA;
                DrawTexturePro(mainRender.texture, src, dest, Vector2.Zero, 0, Color.WHITE);

                Raylib.BeginBlendMode(BlendMode.BLEND_ADD_COLORS);
                Raylib.DrawTexturePro(pingpongFBO[finalBuffer].texture, src, dest, Vector2.Zero, 0, Color.WHITE);
                Raylib.EndBlendMode();


                Raylib.DrawTexturePro(pingpongFBO[finalBuffer].texture, src, new Rectangle(0,0,1280/3f,720/ 3f), Vector2.Zero, 0, Color.WHITE);

                Raylib.DrawFPS(10,10);
                Raylib.EndDrawing();

            }
            Raylib.CloseWindow();


        }

        public static void DrawCube()
        {

            Rlgl.rlPushMatrix();
            for (int i = 0; i < 20; i++)
            {
                Rlgl.rlRotatef((360f/10)*i, 0, 1, 0);

                Raylib.DrawCube(new Vector3(4f, 1f, 0), 0.8f, 2f, 0.8f, Color.RED);
                Raylib.DrawCubeWires(new Vector3(4f, 1f, 0), 0.8f, 2f, 0.8f, Color.WHITE);

            }
            Rlgl.rlPopMatrix();
            //Raylib.DrawPlane(Vector3.Zero,new Vector2(20),Color.GRAY);
        }
        public static void DrawSphere()
        {
            Rlgl.rlPushMatrix();
            Rlgl.rlScalef(10f,10f,10f);


            Raylib.DrawSphere(Vector3.Zero , 0.1f, Color.WHITE);


            Rlgl.rlPopMatrix();

            Rlgl.rlPushMatrix();

            for (int i = 0; i < 10; i++)
            {
                Rlgl.rlRotatef((360f / 10) * i, 0, 1, 0);

                Raylib.DrawCube(new Vector3(4f, 1f, 0), 0.8f, 2f, 0.8f, Color.BLACK);
            }
            Rlgl.rlPopMatrix();
            Raylib.DrawPlane(Vector3.Zero, new Vector2(20), Color.BLACK);

        }

        public static void DrawTextureHDR(Texture2D texture, Rectangle source, Rectangle dest, Vector2 origin, float rotation, Vector4 tint)
        {

            // Check if texture is valid
            if (texture.id > 0)
            {
                float width = (float)texture.width;
                float height = (float)texture.height;

                bool flipX = false;

                if (source.width < 0) { flipX = true; source.width *= -1; }
                if (source.height < 0) source.y -= source.height;

                Vector2 topLeft;
                Vector2 topRight;
                Vector2 bottomLeft;
                Vector2 bottomRight;

                // Only calculate rotation if needed
                if (rotation == 0.0f)
                {
                    float x = dest.x - origin.X;
                    float y = dest.y - origin.Y;
                    topLeft = new Vector2(x, y);
                    topRight = new Vector2(x + dest.width, y);
                    bottomLeft = new Vector2(x, y + dest.height);
                    bottomRight = new Vector2(x + dest.width, y + dest.height);
                }
                else
                {
                    float sinRotation = MathF.Sin(rotation * DEG2RAD);
                    float cosRotation = MathF.Cos(rotation * DEG2RAD);
                    float x = dest.x;
                    float y = dest.y;
                    float dx = -origin.X;
                    float dy = -origin.Y;

                    topLeft.X = x + dx * cosRotation - dy * sinRotation;
                    topLeft.Y = y + dx * sinRotation + dy * cosRotation;

                    topRight.X = x + (dx + dest.width) * cosRotation - dy * sinRotation;
                    topRight.Y = y + (dx + dest.width) * sinRotation + dy * cosRotation;

                    bottomLeft.X = x + dx * cosRotation - (dy + dest.height) * sinRotation;
                    bottomLeft.Y = y + dx * sinRotation + (dy + dest.height) * cosRotation;

                    bottomRight.X = x + (dx + dest.width) * cosRotation - (dy + dest.height) * sinRotation;
                    bottomRight.Y = y + (dx + dest.width) * sinRotation + (dy + dest.height) * cosRotation;
                }

                rlCheckRenderBatchLimit(4);     // Make sure there is enough free space on the batch buffer

                rlSetTexture(texture.id);
                rlBegin(DrawMode.QUADS);

                rlColor4f(tint.X, tint.Y, tint.Z, tint.W);
                rlNormal3f(0.0f, 0.0f, 1.0f);                          // Normal vector pointing towards viewer

                // Top-left corner for texture and quad
                if (flipX) rlTexCoord2f((source.x + source.width) / width, source.y / height);
                else rlTexCoord2f(source.x / width, source.y / height);
                rlVertex2f(topLeft.X, topLeft.Y);

                // Bottom-left corner for texture and quad
                if (flipX) rlTexCoord2f((source.x + source.width) / width, (source.y + source.height) / height);
                else rlTexCoord2f(source.x / width, (source.y + source.height) / height);
                rlVertex2f(bottomLeft.X, bottomLeft.Y);

                // Bottom-right corner for texture and quad
                if (flipX) rlTexCoord2f(source.x / width, (source.y + source.height) / height);
                else rlTexCoord2f((source.x + source.width) / width, (source.y + source.height) / height);
                rlVertex2f(bottomRight.X, bottomRight.Y);

                // Top-right corner for texture and quad
                if (flipX) rlTexCoord2f(source.x / width, source.y / height);
                else rlTexCoord2f((source.x + source.width) / width, source.y / height);
                rlVertex2f(topRight.X, topRight.Y);

                rlEnd();
                rlSetTexture(0);
            }
        }


    }
}