// See https://aka.ms/new-console-template for more information
using Engine;
using Engine.Renderering;
using Engine.SceneManager;
using ImGuiNET;
using Raylib_cs;

using rlImGui_cs;
using System.Numerics;
new App().Run();


public class App : Core
{
    public override void Intitialize()
    {
        base.Intitialize();
        AddManager(new ImguiEntityManager());

        Scene = new Scene3D();
    }
}

public class Scene3D : Scene
{
    public Scene3D() 
        : base("3dWorld", 1280, 720, Color.BLUE, Color.BLACK)
    {
    }
    public override void Begin()
    {
        AddRenderer(new DefaultRenderer3D());
        base.Begin();
    }
    public override void OnBegined()
    {
        CreateEntity("hi")
            .AddComponent(new CubeMesh())
            .AddComponent(new Material());
        CreateEntity("hi2")
           .AddComponent(new CubeMesh())
           .AddComponent(new Material());
        CreateEntity("h3")
           .AddComponent(new CubeMesh())
           .AddComponent(new Material());
    }

    
}

public class Material : Component, ICustomInspectorImgui
{
    public Raylib_cs.Material material = Raylib.LoadMaterialDefault();


    unsafe void ICustomInspectorImgui.OnInspectorGUI()
    {
        foreach (int mapIndex in Enum.GetValues<MaterialMapIndex>().Select(e=>(int)e))
        {
            var map = material.maps[mapIndex];
            ImGui.Text(mapIndex.ToString());
            
            Vector4 c = Raylib.ColorNormalize(map.color);
            ImGui.ColorEdit4($"color##{mapIndex}",ref c);
            material.maps[mapIndex].color = Raylib.ColorFromNormalized(c);
            ImGui.Image((IntPtr)map.texture.id,new Vector2(40),Vector2.Zero,Vector2.One,c,new Vector4(0));
        }
    }
}

public class CubeMesh : RenderableComponent,ICustomInspectorImgui
{
    Mesh mesh = Raylib.GenMeshCube(1,1,1);
    Material? material;

    
    public override void OnAddedToEntity()
    {
        material = Entity.GetComponent<Material>() ;
    }
    public override void Render()
    {
        
        if (material != null)
        {
            Raylib.DrawMesh(mesh,material.material,Matrix4x4.Transpose(Transform.LocalToWorld));
        }
    }

    void ICustomInspectorImgui.OnInspectorGUI()
    {
    }
}