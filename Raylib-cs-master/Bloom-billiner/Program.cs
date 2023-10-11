using Examples;
using Raylib_cs;
using System.Data;
using System.Numerics;
using System.Transactions;



namespace Bloom_billiner
{
    public class Rl
    {
        public static unsafe RenderTexture2D LoadRenderTextureWithDepthTexture(int width, int height)
        {
            RenderTexture2D target = new RenderTexture2D();

            target.id = Rlgl.rlLoadFramebuffer(width, height);   // Load an empty framebuffer

            if (target.id > 0)
            {
                Rlgl.rlEnableFramebuffer(target.id);

                // Create color texture (default to RGBA)
                target.texture.id = Rlgl.rlLoadTexture(null, width, height, PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8, 1);
                target.texture.width = width;
                target.texture.height = height;
                target.texture.format = PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8;
                target.texture.mipmaps = 1;

                // Create depth texture
                target.depth.id = Rlgl.rlLoadTextureDepth(width, height, false);
                target.depth.width = width;
                target.depth.height = height;
                target.depth.format = (PixelFormat)19;       //DEPTH_COMPONENT_24BIT?
                target.depth.mipmaps = 1;

                // Attach color texture and depth texture to FBO
                Rlgl.rlFramebufferAttach(target.id, target.texture.id, FramebufferAttachType.RL_ATTACHMENT_COLOR_CHANNEL0, FramebufferAttachTextureType.RL_ATTACHMENT_TEXTURE2D, 0);
                Rlgl.rlFramebufferAttach(target.id, target.depth.id, FramebufferAttachType.RL_ATTACHMENT_DEPTH, FramebufferAttachTextureType.RL_ATTACHMENT_TEXTURE2D, 0);

                // Check if fbo is complete with attachments (valid)
                if (Rlgl.rlFramebufferComplete(target.id)) 
                    Raylib.TraceLog(TraceLogLevel.LOG_INFO, $"FBO: [ID {target.id}] Framebuffer object created successfully");

                Rlgl.rlDisableFramebuffer();
            }
            else Raylib.TraceLog(TraceLogLevel.LOG_WARNING, "FBO: Framebuffer object can not be created");

            return target;
        }
    }
    
    internal class Program
    {
        static unsafe void Main(string[] args)
        {
            return;
            Raylib.SetConfigFlags(ConfigFlags.FLAG_MSAA_4X_HINT | ConfigFlags.FLAG_WINDOW_RESIZABLE);
            Raylib.InitWindow(1280,720,"Bloom");



            Material dfMat =  Raylib.LoadMaterialDefault();
            Model torus = Raylib.LoadModelFromMesh(Raylib.GenMeshKnot(1,3,50,500)) ;
            Model cube = Raylib.LoadModelFromMesh(Raylib.GenMeshCube(1,1,1)) ;
            Model plane = Raylib.LoadModelFromMesh(Raylib.GenMeshPlane(10,10,3,3));
            Shader shader = Raylib.LoadShader("glsl330/default.vert", "glsl330/default.frag");
            Shader depthshader = Raylib.LoadShader("", "glsl330/depth.fs");


            Camera3D cam3 = new Camera3D(new Vector3(20f,20,-2),Vector3.Zero,Vector3.UnitY,45,CameraProjection.CAMERA_PERSPECTIVE);
            Camera3D camDepth3 = new Camera3D(new Vector3(2f,20,-2),Vector3.Zero,-Vector3.UnitZ,45,CameraProjection.CAMERA_PERSPECTIVE);


            shader.locs[(int)ShaderLocationIndex.SHADER_LOC_VECTOR_VIEW] = Raylib.GetShaderLocation(shader,"viewPos");

            cube.materials[0].shader = shader;
            plane.materials[0].shader = shader;
            torus.materials[0].shader = shader;
            int ambientLoc = Raylib.GetShaderLocation(shader,"ambient");
            float[] ambient = new[] { 0.1f, 0.1f, 0.1f, 1.0f };
            Raylib.SetShaderValue(shader, ambientLoc, ambient, ShaderUniformDataType.SHADER_UNIFORM_VEC4);

            var renderTexture = Raylib.LoadRenderTexture(1280, 720 );
            var depthrenderTexture = Rl.LoadRenderTextureWithDepthTexture(1280, 720);

            Raylib.SetTextureFilter(renderTexture.texture,TextureFilter.TEXTURE_FILTER_TRILINEAR);
            Examples.Light[] lights = new Examples.Light[]
            {
                 Rlights.CreateLight(0,LightType.LIGHT_POINT,new Vector3(0,0.6f,0),Vector3.Zero,Color.RED,shader),
                 //Rlights.CreateLight(1,LightType.LIGHT_POINT,new Vector3(0.2f,0.6f,0),Vector3.Zero,Color.GREEN,shader),
                 //Rlights.CreateLight(2,LightType.LIGHT_POINT,new Vector3(0.2f,0.6f,0),Vector3.Zero,Color.BLUE,shader),
                 //Rlights.CreateLight(3,LightType.LIGHT_POINT,new Vector3(0.2f,0.6f,0),Vector3.Zero,Color.PINK,shader),
            };
            Raylib.SetCameraMode(cam3,CameraMode.CAMERA_FREE);
                plane.materials[0].maps[(int)MaterialMapIndex.MATERIAL_MAP_ALBEDO].texture = renderTexture.texture; 

            int c = 0;
            while(!Raylib.WindowShouldClose())
            {
                Raylib.UpdateCamera(ref cam3);
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_TAB))
                    c = (c + 1) % lights.Length;

                if (Raylib.IsKeyDown(KeyboardKey.KEY_UP)) lights[c].position += Vector3.UnitZ * Raylib.GetFrameTime();
                if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN)) lights[c].position -= Vector3.UnitZ * Raylib.GetFrameTime();
                if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT)) lights[c].position += Vector3.UnitX * Raylib.GetFrameTime();
                if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT)) lights[c].position -= Vector3.UnitX * Raylib.GetFrameTime();
                if (Raylib.IsKeyDown(KeyboardKey.KEY_SPACE)) lights[c].position += Vector3.UnitY * Raylib.GetFrameTime();
                if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT)) lights[c].position -= Vector3.UnitY * Raylib.GetFrameTime();

                var mouse = Raylib.GetMousePosition();
                var screen = new Vector2( Raylib.GetScreenWidth(),Raylib.GetScreenHeight());
                //light0.position = new Vector3(screen.X - mouse.X , 0,screen.Y - mouse.Y);


                Raylib.SetShaderValue(shader,(int)ShaderLocationIndex.SHADER_LOC_MATRIX_VIEW,cam3.position,ShaderUniformDataType.SHADER_UNIFORM_VEC3);
                foreach (var light in lights)
                {
                    //Raylib.SetShaderValue(shader, Raylib.GetShaderLocation(shader, "lightPos"), new float[]{ light.position.X,light.position.Y,light.position.Z },ShaderUniformDataType.SHADER_UNIFORM_VEC3);
                }



                Raylib.BeginTextureMode(depthrenderTexture);
                {
                    Raylib.ClearBackground(Color.BLACK);
                    Raylib.BeginMode3D(cam3);
                }

                #region Render texture
                //Raylib.DrawGrid(10, 0.5f);

                var lightMat = lights[0].Projection * lights[0].View;

                Raylib.DrawModel(cube, Vector3.One * 2, 2, Color.DARKGRAY);
                Raylib.DrawModel(cube, Vector3.UnitZ * 4, 4, Color.DARKGRAY);
                Raylib.DrawModel(plane, Vector3.Zero, 10, Color.GRAY);
                Raylib.DrawModel(torus, Vector3.Zero, 2, Color.GRAY);
                foreach (var light in lights)
                {
                    Raylib.DrawSphere(light.position, 0.01f, light.color);
                }

                Raylib.EndMode3D();

                Raylib.DrawCircleV(Raylib.GetMousePosition(), 20, Color.WHITE); 
                #endregion
                Raylib.EndTextureMode();

                Raylib.SetShaderValue(shader,Raylib.GetShaderLocation(shader,"shadowMap"),renderTexture.depth,ShaderUniformDataType.SHADER_UNIFORM_SAMPLER2D);
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.BLACK);
                Raylib.BeginMode2D(new Camera2D(Vector2.Zero,Vector2.Zero,0,1));


#if false

                Raylib.BeginMode3D(cam3);
                Raylib.DrawGrid(10, 1);

                foreach (var light in lights)
                {
                    Raylib.DrawSphere(light.position, 0.01f, light.color);
                }

                Raylib.DrawModel(cube, Vector3.Zero , 1, Color.YELLOW);
                Raylib.DrawModel(plane, Vector3.Zero , 1, Color.GREEN);


                Raylib.EndMode3D();
                Raylib.DrawTexturePro(renderTexture.texture, new Rectangle(0, 0, 1280, -720), new Rectangle(0, 0, Raylib.GetScreenWidth()/5, Raylib.GetScreenHeight()/5), Vector2.Zero, 0, Color.WHITE);
#else
                //Raylib.BeginShaderMode(depthshader);
                Raylib.DrawTexturePro(depthrenderTexture.texture,new Rectangle(0,0,1280,-720),new Rectangle(0,0,Raylib.GetScreenWidth(),Raylib.GetScreenHeight()),Vector2.Zero,0,Color.WHITE);
                //Raylib.EndShaderMode();
#endif
                Raylib.DrawCircleV(Raylib.GetMousePosition(), 10, Color.RED);

                Raylib.EndMode2D();
                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }

        static void DrawRenderer(ref RenderTexture2D renderTexture2,ref Camera3D camera3D)
        {
            


            
        }
    }
}