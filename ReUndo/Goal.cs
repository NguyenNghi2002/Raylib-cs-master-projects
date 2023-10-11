using Engine;
using Engine.SceneManager;

public class Goal : Component
{
    public VectorInt2 Location;
    private Grid _gridMap;

    public override void OnAddedToEntity()
    {
        if (Scene.TryFindComponent<Grid>(out _gridMap))
        {
            var tile = _gridMap.GetTileFromWorld(Transform.Position2);
            if(tile != null)
            {
                Location.X = tile.X;
                Location.Y = tile.Y;
            }
        }
    }
}
