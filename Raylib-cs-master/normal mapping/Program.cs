using Raylib_cs;
using System.Numerics;

namespace normal_mapping
{
    internal class Program
    {
        static unsafe void Main(string[] args)
        {
            Raylib.SetConfigFlags(ConfigFlags.FLAG_VSYNC_HINT);
            Raylib.InitWindow(1280,720,"normal mapping");

            Model cubemodel = Raylib.LoadModelFromMesh(Raylib.GenMeshCube(0.5f,0.5f,0.5f));
            Raylib.GenMeshTangents(cubemodel.meshes);
            Camera3D lightCam = new Camera3D(new Vector3(1,1,1),Vector3.Zero,Vector3.UnitY,45,CameraProjection.CAMERA_PERSPECTIVE);
            Camera3D cam = new Camera3D(new Vector3(2,2,2),Vector3.Zero,Vector3.UnitY,45,CameraProjection.CAMERA_PERSPECTIVE);
            Raylib.SetCameraMode(lightCam,CameraMode.CAMERA_ORBITAL);

            Shader meshshader = Raylib.LoadShader("mesh.vert","mesh.frag");
            int lightposL = Raylib.GetShaderLocation(meshshader,"lightPos");
            meshshader.locs[(int)ShaderLocationIndex.SHADER_LOC_VECTOR_VIEW] = Raylib.GetShaderLocation(meshshader,"viewPos");
            cubemodel.materials[0].maps[(int)MaterialMapIndex.MATERIAL_MAP_ALBEDO].texture = Raylib.LoadTexture("Plastic_04_basecolor.png");
            cubemodel.materials[0].maps[(int)MaterialMapIndex.MATERIAL_MAP_NORMAL].texture = Raylib.LoadTexture("Plastic_04_normal.png");
            
            Raylib.SetMaterialShader(ref cubemodel,0,ref meshshader);
            while (!Raylib.WindowShouldClose())
            {
                Raylib.UpdateCamera(ref lightCam);
                Raylib.UpdateCamera(ref lightCam);
                Raylib.UpdateCamera(ref lightCam);
                Raylib.UpdateCamera(ref lightCam);

                Raylib.SetShaderValue(meshshader, meshshader.locs[(int)ShaderLocationIndex.SHADER_LOC_VECTOR_VIEW],lightCam.position,ShaderUniformDataType.SHADER_UNIFORM_VEC3);
                Raylib.SetShaderValue(meshshader, lightposL,lightCam.position + Vector3.UnitZ,ShaderUniformDataType.SHADER_UNIFORM_VEC3);

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.GRAY);

                Raylib.BeginMode3D(cam);
                Raylib.DrawModel(cubemodel,Vector3.Zero,1,Color.WHITE);
                Raylib.DrawCube(lightCam.position,0.2f,0.2f,0.2f,Color.YELLOW);
                Raylib.EndMode3D();


                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }

        // Generate a mesh using flags to specify attributes
        unsafe Mesh GenMeshCustom(int vertexCount, int triangleCount, MeshFlag flags)
        {
            Mesh mesh = new Mesh();

            mesh.vertexCount = vertexCount;
            mesh.triangleCount = triangleCount;
            
            Raylib.TraceLog(TraceLogLevel.LOG_INFO, $"GenMeshCustom: vertexCount={mesh.vertexCount} triangleCount={mesh.triangleCount}" );

            if ((flags & MeshFlag.MESH_VERTEX) == 0)
            {
                mesh.vertices = (float*)Raylib.MemAlloc(mesh.vertexCount * 3 * sizeof(float));
            }
            if ((flags & MeshFlag.MESH_TEXCOORD)== 0)
            {
                
                mesh.texcoords = (float*)Raylib.MemAlloc(mesh.vertexCount * 2 * sizeof(float));
            }
            if ((flags & MeshFlag.MESH_TEXCOORD2)== 0)
            {
                mesh.texcoords2 = (float*)Raylib.MemAlloc(mesh.vertexCount * 2 * sizeof(float));
            }
            if ((flags & MeshFlag.MESH_NORMAL)== 0)
            {
                mesh.normals = (float*)Raylib.MemAlloc(mesh.vertexCount * 3 * sizeof(float));
            }
            if ((flags & MeshFlag.MESH_TANGENT)== 0)
            {
                mesh.tangents = (float*)Raylib.MemAlloc(mesh.vertexCount * 4 * sizeof(float));
            }
            if ((flags & MeshFlag.MESH_COLOR) == 0)
            {
                mesh.colors = (byte*)Raylib.MemAlloc(mesh.vertexCount * 4 * sizeof(float));
            }
            if ((flags & MeshFlag.MESH_INDEX)== 0)
            {
                mesh.indices = (ushort*)Raylib.MemAlloc(mesh.triangleCount * 3 * sizeof(float));
            }

            mesh.vboId = (uint*)Raylib.MemRealloc((void*)7, sizeof(uint));

            return mesh;
        }
        [Flags]
        public enum MeshFlag {
            MESH_VERTEX,
            MESH_TEXCOORD,
            MESH_TEXCOORD2,
            MESH_NORMAL,
            MESH_TANGENT,
            MESH_COLOR,
            MESH_INDEX
        }
    }
}