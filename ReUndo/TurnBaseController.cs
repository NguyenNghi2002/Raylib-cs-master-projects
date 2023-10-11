using Engine;
using Engine.External;
using Engine.SceneManager;
using Engine.Texturepacker;
using Engine.TiledSharp;
using Engine.UI;
using Raylib_cs;
using System.Numerics;
public class TurnBaseController : Component,IUpdatable
{
    CommandSystem _movementSystem;
    Grid _grid;
    public VectorInt2 CurrentLocation;

    public Func<VectorInt2,bool> MoveCondition;
    public event Action OnMoved;
    public event Action OnUndoed;
    public event Action OnRedoed;

    public float moveGap = 64f;
    public KeyboardKey 
        UpKey = KeyboardKey.KEY_UP,
        DownKey = KeyboardKey.KEY_DOWN,
        LeftKey = KeyboardKey.KEY_LEFT,
        RightKey = KeyboardKey.KEY_RIGHT;
    public bool EnableControlMove { get; set; } = true;
    public bool EnableCommandMove { get; set; } = true;

    public int UpdateOrder { get; set; }

    public override void OnAddedToEntity()
    {
        if(Entity.TryGetComponent(out _movementSystem)) { }
        if(Scene.TryFindComponent<Grid>(out _grid))
        {
            CurrentLocation =  _grid.GetLocation(Transform.Position2);
        }

    }
    public override void OnTransformChanged(Transformation.Component component)
    {
            CurrentLocation =  _grid.GetLocation(Transform.Position2);
    }
    public void Update()
    {
        if(_movementSystem != null)
        {
            if (EnableCommandMove)
            {
                bool isZPress = Input.IsKeyPressed(KeyboardKey.KEY_Z);
                bool isCtrlDown = InputUtils.IsControlDown();
                bool isShiftDown = InputUtils.IsShiftDown();
                if (isCtrlDown && isShiftDown && isZPress)
                {


                    if (_movementSystem.SendRedoCommand())
                    {
                        UpdateLocation();
                        OnRedoed?.Invoke();
                    }
                }
                else if (isCtrlDown && isZPress)
                {
                    if (_movementSystem.SendUndoCommand())
                    {
                        UpdateLocation();
                        OnUndoed?.Invoke();
                    }

                } 
            }

            if (EnableControlMove)
            {
                if (Input.IsKeyReleased(UpKey)) DoMove(-Vector2.UnitY);
                if (Input.IsKeyReleased(DownKey)) DoMove(Vector2.UnitY);
                if (Input.IsKeyReleased(LeftKey)) DoMove(-Vector2.UnitX);
                if (Input.IsKeyReleased(RightKey)) DoMove(Vector2.UnitX); 
            }

        }
        else
        {
            if (Input.IsKeyReleased(UpKey)) Transform.Position2 -= Vector2.UnitY * moveGap;
            if (Input.IsKeyReleased(DownKey)) Transform.Position2 += Vector2.UnitY * moveGap;
            if (Input.IsKeyReleased(LeftKey)) Transform.Position2 -= Vector2.UnitX * moveGap;
            if (Input.IsKeyReleased(RightKey)) Transform.Position2 += Vector2.UnitX * moveGap;
        }
        
    }
    

    void UpdateLocation()
    {
        var tile = _grid.GetTileFromWorld(Transform.Position2 );
        if (tile == null || tile.Gid != 0)
        {
            return;
        }

        CurrentLocation.X = tile.X;
        CurrentLocation.Y = tile.Y;
    }
    void DoMove(Vector2 direction)
    {
        if (direction == default(Vector2)) return;
        
        var toLocation = _grid.GetLocation(Transform.Position2 + direction * moveGap);


        var isValidLocation = 
            toLocation.X >= 0 && toLocation.X < _grid._gridLayer.Map.Width &&
            toLocation.Y >= 0 && toLocation.Y < _grid._gridLayer.Map.Height
            ;
        if (!isValidLocation)  //Pervent go outside the boudaries
            return;

        
        if (_grid.GetTile(toLocation).Gid != 0) //check if Gid not empty
            return;

        //Execute command
        _movementSystem.ExecuteCommand(new MoveCommand(Transform, direction, moveGap));
        UpdateLocation();
        
        OnMoved?.Invoke();
        Console.WriteLine( CurrentLocation);
    }

}
