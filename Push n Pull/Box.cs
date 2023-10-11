
using Engine;
using Engine.Velcro;
using Genbox.VelcroPhysics.Collision;
using Genbox.VelcroPhysics.Shared;

public class BoxGoal : Component
{
    public List<Box> Interactors = new List<Box>();

    public override void OnAddedToEntity()
    {
        Scene.GetOrCreateSceneComponent<BoxesManager>().boxeGoals.Add(this);
    }

    public override void OnRemovedFromEntity()
    {
        Scene.GetOrCreateSceneComponent<BoxesManager>().boxeGoals.Remove(this);
    }
}

public class Box : Component
{
    BoxesManager BoxesManager;
    List<BoxGoal> Interactors = new List<BoxGoal>();

    public override void OnAddedToEntity()
    {
        BoxesManager = Scene.GetOrCreateSceneComponent<BoxesManager>();
        BoxesManager.boxes.Add(this) ;
    }

    public override void OnRemovedFromEntity()
    {
        BoxesManager.boxes.Remove(this);
    }
    public override void OnTransformChanged(Transformation.Component component)
    {

#if false
        if (component is Transformation.Component.Position && Interactors.Count == 0)
        {

            Entity.GetComponent<VCollisionBox>().RawFixture.GetAABB(out AABB aabb, 0);

            Scene.GetOrCreateSceneComponent<VWorld2D>().World.QueryAABB((f =>
            {
                if ((f.UserData as Entity).TryGetComponent<BoxGoal>(out var boxGoalcpn))
                {
                    boxGoalcpn.Interactors.Add(this);
                    this.Interactors.Add(boxGoalcpn);
                    Console.WriteLine(Interactors.Count);
                }

                return true;
            }), ref aabb);

        } 
#endif
    }
}
