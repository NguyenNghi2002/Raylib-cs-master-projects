using Engine;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System.Numerics;
using System.Reflection;
using static ImGuiNET.ImGui;
public interface IInspectorGUI
{
    void OnInspectorGUI();
}
public class EntityManager : GlobalManager
{
    int entityIndex = 0;
    ImGuiIOPtr _io;
    ImGuiStylePtr _style;
    public EntityManager()
    {
        rlImGui.Setup(true);
        _io = ImGui.GetIO();
        _style = ImGui.GetStyle();
        SetGreenStyle();
    }
    public static void SetGreenStyle()
    {
        var style = ImGui.GetStyle();
        var cols = style.Colors;

        var PhthaloGreen = new Vector4(1, 38, 34, 255) / 255;
        var RichBlack = new Vector4(0, 59, 54, 255) / 255;
        var Magnolia = new Vector4(236, 229, 240, 255) / 255;
        var Fulvous = new Vector4(233, 138, 21, 255) / 255;
        var PalatinatePurple = new Vector4(89, 17, 77, 255) / 255;
        var lightGreen = new Vector4(199, 249, 204, 255) / 255;

        style.FrameBorderSize = 1f;

        cols[(int)ImGuiCol.WindowBg] = new Vector4(1,38,34,130)/255 ;
        cols[(int)ImGuiCol.Border] = Magnolia;
        cols[(int)ImGuiCol.Text] = Magnolia;
        cols[(int)ImGuiCol.TitleBg] = RichBlack;
        cols[(int)ImGuiCol.CheckMark] = lightGreen;
        cols[(int)ImGuiCol.Tab] = Fulvous;
        cols[(int)ImGuiCol.FrameBg] = PhthaloGreen;
        cols[(int)ImGuiCol.PlotLines] = Fulvous;

        cols[(int)ImGuiCol.TabActive] = RichBlack;
        cols[(int)ImGuiCol.TitleBgActive] = Fulvous;
        cols[(int)ImGuiCol.FrameBgActive] = Fulvous;
    }
    protected override void OnDrawDebug()
    {
        var entities = Core.Scene.EntityList;
        rlImGui.Begin();
        ImGui.ListBox("Scene", ref entityIndex, entities.Select(e => e.Name).ToArray(), entities.Count);
        if (ImGui.Button("Add"))
        {
            Core.Scene.CreateEntity("a");
        }

        ShowInspector(entities.ElementAt(entityIndex));
        ImGui.ShowStyleEditor();
        


        rlImGui.End();
    }


    void ShowInspector(Entity select)
    {
        var inputHints = ImGuiInputTextFlags.AutoSelectAll;
        var inspectorHints = ImGuiWindowFlags.None;
        
        if (select != null && ImGui.Begin($"Inspector", inspectorHints))
        {
            var inspectorWidth = ImGui.GetWindowWidth();
            ImGui.InputText("Name", ref select.Name, 100, inputHints);


            if (ImGui.TreeNodeEx("Tranform", ImGuiTreeNodeFlags.DefaultOpen))
            {

                ImGui.PushItemWidth(inspectorWidth / 3 - _style.IndentSpacing - _style.WindowPadding.X - _style.ItemSpacing.X);
                var tf = select.Transform;

                //position
                var prevPos = tf.LocalPosition;
                var pos = prevPos;

                ImGui.Text("Position");
                ImGui.DragFloat("X##p", ref pos.X); SameLine();
                ImGui.DragFloat("Y##p", ref pos.Y); SameLine();
                ImGui.DragFloat("Z##p", ref pos.Z);
                if (pos != prevPos)
                    tf.LocalPosition = pos;

                //scale
                var prevSca = tf.LocalScale;
                var sca = prevSca;

                ImGui.Text("Scale");

                ImGui.DragFloat("X##s", ref sca.X); SameLine();
                ImGui.DragFloat("Y##s", ref sca.Y); SameLine();
                ImGui.DragFloat("Z##s", ref sca.Z);

                if (sca != prevSca)
                    tf.LocalScale = sca;

                //rotation
                var prevRot = tf.EulerLocalRotation * Raylib.RAD2DEG;
                var rot = prevRot;

                ImGui.Text("Rotation");

                ImGui.DragFloat("X##r", ref rot.X); SameLine();
                ImGui.DragFloat("Y##r", ref rot.Y); SameLine();
                ImGui.DragFloat("Z##r", ref rot.Z);

                if (rot != prevRot)
                    tf.EulerLocalRotation = rot * Raylib.DEG2RAD;

                ImGui.PopItemWidth();
                ImGui.TreePop();
            }

            foreach (var component in select.Components)
            {
                if (ImGui.TreeNodeEx(component.GetType().Name))
                {
                    if (component is IInspectorGUI cpn) cpn.OnInspectorGUI();
                    ImGui.TreePop();
                }
            }
            ImGui.End();
        }
        var ass = Assembly.GetAssembly(typeof(Component));
        foreach (var item in ass.GetTypes().Where(t=>t.IsSubclassOf(typeof(Component))))
        {
            if (ImGui.Button(item.FullName))
            {
                var a = (Component)(item.FullName);
                select.AddComponent(a);
            }
        }
    }




    ~EntityManager()
    {
        rlImGui.Shutdown();
    }
}