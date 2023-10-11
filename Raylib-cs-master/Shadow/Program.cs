using System.Numerics;
using System.Runtime;
using Raylib_cs;

namespace Shadow
{
    internal class Program
    {

        static unsafe void Main(string[] args)
        {
            Raylib.SetConfigFlags(ConfigFlags.FLAG_MSAA_4X_HINT | ConfigFlags.FLAG_WINDOW_RESIZABLE);
            Raylib.InitWindow(1280, 720, "shadow");
            Raylib.EnableCursor();
            Raylib.SetTargetFPS(60); 

            float aspect = (float)Raylib.GetScreenWidth() / (float)Raylib.GetScreenHeight();
            Vector3 lightPos = new Vector3(2, 5, 4);

            RenderTexture2D shadowMap = Rl.LoadRenderTextureWithDepthTexture(1024,1024);
            

            //Shaders
            Shader shader_shadow = Raylib.LoadShader("glsl330/shadow.vs", "glsl330/shadow.fs");

            int shader_shadow_matShadow = Raylib.GetShaderLocation(shader_shadow, "matShadow");
            shader_shadow.locs[9] = Raylib.GetShaderLocation(shader_shadow, "matModel");

            Shader shader_mesh = Raylib.LoadShader("glsl330/geom.vert", "glsl330/geom.frag");
            
            shader_mesh.locs[11] = Raylib.GetShaderLocation(shader_mesh,"viewPos");
            shader_mesh.locs[9] = Raylib.GetShaderLocation(shader_mesh,"matModel");


            int shader_mesh_lightPos = Raylib.GetShaderLocation(shader_mesh, "lightPos");
            int shader_mesh_lightDir = Raylib.GetShaderLocation(shader_mesh, "lightDir");
            int shader_mesh_lightColor = Raylib.GetShaderLocation(shader_mesh, "lightColor");
            int shader_mesh_matShadow = Raylib.GetShaderLocation(shader_mesh, "matShadow");
            int shader_mesh_shadowMap = Raylib.GetShaderLocation(shader_mesh, "shadowMap");

            Shader shader_depth = Raylib.LoadShader("", "glsl330/depth.fs");

            Raylib.SetShaderValue(shader_mesh, shader_mesh_lightColor, new float[] { 0.7f,0.7f,0.7f}, ShaderUniformDataType.SHADER_UNIFORM_VEC3); ;
            //Models
            List<Model> models = new List<Model>()
            {
                //Raylib.LoadModelFromMesh(Raylib.GenMeshKnot(4, 3, 50, 500)),
                Raylib.LoadModelFromMesh(Raylib.GenMeshCube(4, 1, 1)),
                Raylib.LoadModelFromMesh(Raylib.GenMeshCube(1, 4, 1)),
                Raylib.LoadModelFromMesh(Raylib.GenMeshPlane(300, 300, 3, 3)),
            };

            var txt = Raylib.LoadTexture("texture3.png");
            foreach (var model in models)
            {
                model.materials[0].shader = shader_mesh;
                model.materials[0].maps[0].texture = txt ;
            }

            //Camera
            var cam3 = new Camera3D(new Vector3(10f, 10, 0), new Vector3(0),Vector3.UnitY, 45f,CameraProjection.CAMERA_PERSPECTIVE);
            var caster = new Camera3D(lightPos, Vector3.Zero,Vector3.UnitY,45, CameraProjection.CAMERA_ORTHOGRAPHIC);
            Raylib.SetCameraMode(cam3,CameraMode.CAMERA_FIRST_PERSON);


            while (!Raylib.WindowShouldClose())
            {
                Raylib.UpdateCamera(ref cam3);

                float speed = 10;
                #region Control
                if (Raylib.IsKeyDown(KeyboardKey.KEY_UP)) lightPos += Vector3.UnitZ * speed * Raylib.GetFrameTime();
                if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN)) lightPos -= Vector3.UnitZ * speed * Raylib.GetFrameTime();
                if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT)) lightPos += Vector3.UnitX * speed * Raylib.GetFrameTime();
                if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT)) lightPos -= Vector3.UnitX * speed * Raylib.GetFrameTime();
                if (Raylib.IsKeyDown(KeyboardKey.KEY_SPACE)) lightPos += Vector3.UnitY * speed * Raylib.GetFrameTime();
                if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT)) lightPos -= Vector3.UnitY * speed * Raylib.GetFrameTime();
                caster.position = lightPos;


                #endregion

                Matrix4x4 view =  Raylib.GetCameraMatrix(caster);
                Matrix4x4 proj = Raymath.MatrixOrtho(-50, +50, -50, 50, 1f, 10);
                //Matrix4x4 proj = Raymath.MatrixFrustum(-20, +20, -20, +20, 0.01f, 1000);
                Matrix4x4 matShadow =  view * proj;
                Raylib.SetShaderValueMatrix(shader_shadow, shader_shadow_matShadow, matShadow);


                Raylib.BeginTextureMode(shadowMap);
                Raylib.ClearBackground(Color.WHITE);
                Raylib.BeginMode3D(caster);

                //Render Scene
                DrawScene(models,shader_shadow);

                Raylib.EndMode3D();
                Raylib.EndTextureMode();
                //-------------------------------------------------------------------


                //Update ShaderMesh
                Raylib.SetShaderValue(shader_mesh, shader_mesh.locs[11], cam3.position, ShaderUniformDataType.SHADER_UNIFORM_VEC3);
                Raylib.SetShaderValue(shader_mesh, shader_mesh_lightPos, caster.position, ShaderUniformDataType.SHADER_UNIFORM_VEC3);
                Raylib.SetShaderValueMatrix(shader_mesh, shader_mesh_matShadow, matShadow);



                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.DARKGRAY);
                Raylib.BeginMode3D(cam3);
                //Render Scene
                DrawScene(models,shader_mesh);
                

                Raylib.DrawSphere(caster.position,0.1f,Color.WHITE);
                Raylib.DrawSphereWires(caster.position,0.4f,4,4,Color.WHITE);
                Raylib.DrawCylinderWiresEx(caster.position,caster.target,5f,5f,4,Color.RED);

                Raylib.EndMode3D();



                Raylib.BeginShaderMode(shader_depth);
                Raylib.DrawTexturePro(shadowMap.depth, new Rectangle(0, 0, 1280,  -720), new Rectangle(0, 0, shadowMap.depth.width/4f, shadowMap.depth.height/4f), Vector2.Zero, 0, Color.WHITE);
                Raylib.EndShaderMode() ;

                Raylib.SetShaderValueTexture(shader_mesh, shader_mesh_shadowMap, shadowMap.depth);


                Raylib.EndDrawing();

            }

            Console.WriteLine("Hello, World!");
        }

        static unsafe void DrawScene(List<Model> models,Shader shader)
        {
            for (int i = models.Count() - 1; i >= 0; i--)
            {

                models[i].materials[0].shader = shader;
                Raylib.DrawModel(models[i], -Vector3.UnitY * (i) * 2, 1, Color.GRAY); ;
            }
        }
    }

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

                Raylib.SetTextureWrap(target.depth,TextureWrap.TEXTURE_WRAP_CLAMP);
                Raylib.SetTextureFilter(target.depth,TextureFilter.TEXTURE_FILTER_BILINEAR);

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

}