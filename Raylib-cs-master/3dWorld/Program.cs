using OpenGL;
using Raylib_cs;
using System.Numerics;
using System.Reflection.Metadata;
using static Raylib_cs.Raylib;
using static Raylib_cs.ShaderLocationIndex;

namespace _3dWorld
{
#if true
    internal unsafe class Program
    {
        static Material defaultMaterial = Raylib.LoadMaterialDefault();
        static List<Model> primitives = new List<Model>()
        {
            Raylib.LoadModelFromMesh(Raylib.GenMeshPlane(10f, 10f, 1, 1)),
            //Raylib.LoadModel("watermill.obj"),
            Raylib.LoadModelFromMesh(Raylib.GenMeshCube(1f, 1f, 1f)),
            //Raylib.LoadModelFromMesh(Raylib.GenMeshCube(0.1f, 2.1f, 0.1f)),
            //Raylib.LoadModelFromMesh(Raylib.GenMeshCylinder(0.05f, 3, 10)),
            //Raylib.LoadModelFromMesh(Raylib.GenMeshSphere(0.05f, 40, 40)),
            //Raylib.LoadModelFromMesh(Raylib.GenMeshKnot(2f, 0.2f, 90, 100)),
        };
        static Texture2D caroTxt;
        static Model skyboxModel;
        static void Main(string[] args)
        {

                //Configuration
                Raylib.SetConfigFlags(ConfigFlags.FLAG_VSYNC_HINT | ConfigFlags.FLAG_MSAA_4X_HINT);
            Raylib.InitWindow(1280, 720, "World");
            Raylib.SetWindowPosition(0, 0);
            var aspect = GetScreenWidth() / GetScreenHeight();

            
            //Setup skybox

            skyboxModel = LoadModelFromMesh(GenMeshCube(1,1,1));
            Shader skyboxShader = Raylib.LoadShader("skybox.vert","skybox.frag");
            SetMaterialShader(ref skyboxModel,0,ref skyboxShader);
            skyboxShader.locs[(int)SHADER_LOC_MAP_CUBEMAP] = GetShaderLocation(skyboxShader, "environmentMap");
            SetShaderValue(skyboxShader,GetShaderLocation(skyboxShader,"doGamma"),0,ShaderUniformDataType.SHADER_UNIFORM_INT);
            SetShaderValue(skyboxShader,GetShaderLocation(skyboxShader,"vflipped"),0,ShaderUniformDataType.SHADER_UNIFORM_INT);
            Shader cubeShader = Raylib.LoadShader("cubemap.vert","cubemap.frag");
            cubeShader.locs[(int)SHADER_LOC_COLOR_DIFFUSE] = GetShaderLocation(cubeShader, "equirectangularMap");
            //SetShaderValue(cubeShader,GetShaderLocation(cubeShader,"equirectangularMap"),0,ShaderUniformDataType.SHADER_UNIFORM_INT);

            //Image skyboxImg = LoadImage("dresden_square_2k.hdr");
            //Raylib.ImageFormat(ref skyboxImg,PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8);
            Image skyboxImg = LoadImage("skybox.png");
            Texture2D skyboxTxt = LoadTextureFromImage(skyboxImg);
            Texture2D skyboxCubeMapTxt = LoadTextureCubemap(skyboxImg, CubemapLayout.CUBEMAP_LAYOUT_AUTO_DETECT);
            skyboxModel.materials[0].maps[(int)MaterialMapIndex.MATERIAL_MAP_CUBEMAP].texture = skyboxCubeMapTxt;
            //skyboxModel.materials[0].maps[(int)MaterialMapIndex.MATERIAL_MAP_CUBEMAP].texture = skyboxTxt;
            //skyboxModel.materials[0].maps[(int)MaterialMapIndex.MATERIAL_MAP_CUBEMAP].texture = skyboxTxt;


            Image checkedImg = Raylib.GenImageChecked(10, 10, 2, 2, Color.RED, Color.YELLOW);
            caroTxt = Raylib.LoadTextureFromImage(checkedImg);
            Shader meshShader = Raylib.LoadShader("mesh.vert", "mesh.frag");
            int mesh_matLightLoc = Raylib.GetShaderLocation(meshShader, "matLight");
            int mesh_lightPosLoc = Raylib.GetShaderLocation(meshShader, "lightPos");
            meshShader.locs[(int)SHADER_LOC_VECTOR_VIEW] = Raylib.GetShaderLocation(meshShader, "viewPos");
            meshShader.locs[(int)SHADER_LOC_MAP_CUBEMAP] = Raylib.GetShaderLocation(meshShader, "cubeMap");
            meshShader.locs[(int)SHADER_LOC_MAP_NORMAL] = Raylib.GetShaderLocation(meshShader, "normalMap");
            meshShader.locs[(int)SHADER_LOC_MAP_METALNESS] = Raylib.GetShaderLocation(meshShader, "metalnessMap");
            int mesh_shadowMapLoc = Raylib.GetShaderLocation(meshShader, "shadowMap");
            Shader depthShader = Raylib.LoadShader(null, "depth.frag");
            Shader shadowShader = Raylib.LoadShader("shadow.vert", "shadow.frag");
            int shadow_matLightLoc = Raylib.GetShaderLocation(shadowShader, "matLight");

            Camera3D caster = new Camera3D(new Vector3(2f, 0.4f, 2f), Vector3.Zero, Vector3.UnitY, 45, CameraProjection.CAMERA_PERSPECTIVE);
            Camera3D cam = new Camera3D(new Vector3(2, 0.4f, -2f), new Vector3(), Vector3.UnitY, 45, CameraProjection.CAMERA_PERSPECTIVE);
            Raylib.SetCameraMode(cam, CameraMode.CAMERA_FIRST_PERSON);

            Texture2D dif = Raylib.LoadTexture("Plastic_04/Plastic_04_basecolor.png"); 
            Texture2D normal = Raylib.LoadTexture("Plastic_04/Plastic_04_normal.png"); 
            Raylib.GenMeshTangents(primitives[0].meshes);
            foreach (var p in primitives)
            {
                Raylib.SetMaterialTexture(ref p.materials[0],MaterialMapIndex.MATERIAL_MAP_ALBEDO,dif);
                //Raylib.SetMaterialTexture(ref p.materials[0],MaterialMapIndex.MATERIAL_MAP_NORMAL,normal);
                Raylib.SetMaterialTexture(ref p.materials[0],MaterialMapIndex.MATERIAL_MAP_CUBEMAP,skyboxCubeMapTxt);
            }
            //Raylib.SetMaterialTexture(ref primitives[1].materials[0],MaterialMapIndex.MATERIAL_MAP_ALBEDO,LoadTexture("watermill_diffuse.png"));
            RenderTexture2D shadowMap = LoadRenderTextureWithDepthTexture(Raylib.GetScreenWidth() , Raylib.GetScreenHeight() );


            while (!WindowShouldClose())
            {
                #region light control
                var speed = 10;
                var lightPos = caster.position;
                if (Raylib.IsKeyDown(KeyboardKey.KEY_UP)) lightPos += Vector3.UnitZ * speed * Raylib.GetFrameTime();
                if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN)) lightPos -= Vector3.UnitZ * speed * Raylib.GetFrameTime();
                if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT)) lightPos += Vector3.UnitX * speed * Raylib.GetFrameTime();
                if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT)) lightPos -= Vector3.UnitX * speed * Raylib.GetFrameTime();
                if (Raylib.IsKeyDown(KeyboardKey.KEY_EQUAL)) lightPos += Vector3.UnitY * speed * Raylib.GetFrameTime();
                if (Raylib.IsKeyDown(KeyboardKey.KEY_MINUS)) lightPos -= Vector3.UnitY * speed * Raylib.GetFrameTime();
                caster.position = lightPos; 
                #endregion

                Raylib.UpdateCamera(ref cam);
                Matrix4x4 view = Raylib.GetCameraMatrix(caster);
                //Matrix4x4 proj = Raymath.MatrixOrtho(-10,10,-10,10,0.1f,7.5f); // ortho
                Matrix4x4 proj = Raymath.MatrixPerspective(caster.fovy * Raylib.DEG2RAD, aspect, 0.01f, 1000f);
                Matrix4x4 matLight = proj * view;

                //begin draw depth
                GLHelper.glCullFace(GLHelper.GL_FRONT);
                Raylib.BeginTextureMode(shadowMap);
                Rlgl.rlClearScreenBuffers();
                Raylib.BeginMode3D(caster);

                //Update light matrix in shadow shader
                Raylib.SetShaderValueMatrix(shadowShader, shadow_matLightLoc, matLight);

                DrawScence(shadowShader);

                //end depth map
                Raylib.EndMode3D();
                Raylib.EndTextureMode();
                GLHelper.glCullFace(GLHelper.GL_BACK);


                BeginDrawing();
                ClearBackground(Color.BLANK); ;
                Raylib.EnableCursor();

#if true
                Raylib.BeginMode3D(cam);

                Raylib.SetShaderValue(meshShader, meshShader.locs[(int)SHADER_LOC_VECTOR_VIEW], cam.position, ShaderUniformDataType.SHADER_UNIFORM_VEC3);
                Raylib.SetShaderValue(meshShader, mesh_lightPosLoc, caster.position, ShaderUniformDataType.SHADER_UNIFORM_VEC3);
                Raylib.SetShaderValueMatrix(meshShader, mesh_matLightLoc, matLight);
                Raylib.SetShaderValueTexture(meshShader, mesh_shadowMapLoc, shadowMap.depth);

                

                DrawGrid(10, 1f);


                DrawScence(meshShader);
                Rlgl.rlDisableBackfaceCulling();

                GLHelper.glDepthFunc(GLHelper.GL_LEQUAL);
                DrawModel(skyboxModel, Vector3.Zero, 1, Color.WHITE);
                GLHelper.glDepthFunc(GLHelper.GL_LESS);

                Rlgl.rlEnableBackfaceCulling();

                Raylib.DrawSphere(caster.position, 0.1f, Color.WHITE);
                Rlgl.rlSetLineWidth(2f);
                Raylib.DrawCylinderWiresEx(caster.position,caster.target,1f,2f,4,Color.RED);
                

                Raylib.EndMode3D();

#endif


                Raylib.BeginShaderMode(depthShader);
                Raylib.DrawTexturePro(shadowMap.depth, new Rectangle(0, 0, shadowMap.depth.width, -shadowMap.depth.height), new Rectangle(0, 0, shadowMap.depth.width / 2f, shadowMap.depth.height/2f), Vector2.Zero, 0, Color.WHITE);
                Raylib.EndShaderMode();

                EndDrawing();
            }

                //CloseWindow();
        }
        public static unsafe void DrawScence(Shader shader)
        {
            
            var a = 0;
            foreach (var obj in primitives)
            {
                obj.materials[0].shader = shader;
                Raylib.DrawModel(obj, new Vector3(a++,0,0), 1, Color.WHITE); ;
            }
        }

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

                Raylib.SetTextureWrap(target.depth, TextureWrap.TEXTURE_WRAP_CLAMP);
                Raylib.SetTextureFilter(target.depth, TextureFilter.TEXTURE_FILTER_BILINEAR);

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
#endif
}