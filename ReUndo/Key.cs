using Engine;
using Engine.SceneManager;
using System.Security.Cryptography;

public class Key : Component
{
    Player _player;
    Grid? _gridmap;
    Door _door;

    public int _keyID ;
    public VectorInt2 Location;
    public event Action OnActivate;

    public Key(int id = -1)
    {
        SetID(id);
    }

    public override void OnTransformChanged(Transformation.Component component)
    {
        if (component == Transformation.Component.Position )
        {
            UpdateLocation();
        }
    }
    public override void OnAddedToEntity()
    {
        //update key location
        if(Scene.TryFindComponent<Grid>(out _gridmap))
        {
            UpdateLocation();
        }

        //Setup events
        if(Scene.TryFindComponent<Player>(out _player))
        {
            _player._controller.OnMoved += OnTouched;
            _player._controller.OnUndoed += OnTouched;
            _player._controller.OnRedoed += OnTouched;
        }
        _door = Scene.FindComponents<Door>().Find(d => d._doorID == _keyID);

    }


    void UpdateLocation()
    {
        if (_gridmap == null) return;
        var tile = _gridmap.GetTileFromWorld(Transform.Position2);
        Location =  new VectorInt2(tile.X, tile.Y);

    }

    void OnTouched()
    {
        if (Location == null) return;

        if (Location == _player._controller.CurrentLocation)
        {
            _gridmap.GetTile(_door.Location).Gid = 0;
            _door.Entity.Enable = false;
            Entity.Destroy(this.Entity);

            Console.WriteLine("Hit key");
        }
    }

    public void SetID(int id)
    {
        _keyID = id;
    }

    public override void OnRemovedFromEntity()
    {
        _player._controller.OnMoved -= OnTouched;
        _player._controller.OnUndoed -= OnTouched;
        _player._controller.OnRedoed -= OnTouched;
    }

}
