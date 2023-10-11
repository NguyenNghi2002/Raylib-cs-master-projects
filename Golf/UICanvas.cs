using Engine;
using Engine.SceneManager;
using Engine.UI;
using Raylib_cs;
namespace Golf
{
    public class UICanvas : RenderableComponent,IUpdatable
    {
        public Stage Stage { get; } = new Stage();
        public int UpdateOrder { get; set; } = 0;

        public override void OnAddedToEntity()
        {
            Stage.Entity = Entity;
        }
        public override void Render()
        {
            var off = Scene.Camera.offset ;
            var target = Scene.Camera.target ;
            var zoom = 1f/Scene.Camera.zoom;
            Rlgl.rlPushMatrix();

            Rlgl.rlTranslatef(off.X,off.Y,0f);
            Rlgl.rlTranslatef(target.X,target.Y, 0f);
            Rlgl.rlScalef(zoom,zoom,1f);

            Stage.Render(Scene.Camera);
            Rlgl.rlPopMatrix();
            Raylib.DrawCircleV(Stage.GetMousePosition(),2,Color.RED);
            Raylib.DrawCircleV(Input.GetScaledMousePosition(),2,Color.GREEN);
        }

        public override void OnRemovedFromEntity()
        {
            Stage.Dispose();
        }

        public void Update()
        {
            Stage.Update();
        }
    }



}