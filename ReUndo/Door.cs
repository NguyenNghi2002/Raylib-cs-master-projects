using Engine;
using Engine.SceneManager;

public class Door : Component
{
    public int _doorID;
    Grid _grid;
    Player _player;

    public VectorInt2 Location;

    public Door(int id)
    {
        this._doorID = id;
    }
    public override void OnAddedToEntity()
    {
        if (Scene.TryFindComponent<Player>(out _player) 
            && Scene.TryFindComponent<Grid>(out _grid))
        {
            _player._controller.MoveCondition += (toLoc) =>
            {
                return toLoc != _grid.GetLocation(this.Transform.Position2);
            };
        }
        Location =  _grid.GetLocation(Transform.Position2);


        foreach (var key in Scene.FindComponents<Key>())
        {
            key.OnActivate += () => Entity.Destroy(this.Entity);
        }
    }


    public override void OnTransformChanged(Transformation.Component component)
    {
        if(component == Transformation.Component.Position)
            Location = _grid.GetLocation(Transform.Position2);
    }
}
