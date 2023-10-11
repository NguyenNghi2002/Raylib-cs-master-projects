using Engine;
using Engine.External;
using Engine.SceneManager;
using Engine.UI;
public class Player : Component
{
    public TurnBaseController _controller;
    TileMap _tilemap;

    public int MoveCount
    {
        get => _moveCount;
        set => SetMoveCount(value);
    }

    int _moveCount = 10;
    public Label moveCountLabel;

    public override void OnAddedToEntity()
    {
        if (Entity.TryGetComponent(out _controller))
        {
            _controller.OnMoved += () =>
            {
                --MoveCount;
                UpdateStatus();
            };
            _controller.OnUndoed += UpdateStatus; ;
            _controller.OnRedoed += UpdateStatus ;
            _controller.EnableControlMove = MoveCount > 0;
        }

        var gm = Scene.GetOrCreateSceneComponent<GameManager>();
        if (gm != null)
        {
            var tb = new Table()
                .Top().Left()
                .SetFillParent(true)
                .Pad(20);
                ;

            moveCountLabel = new Label(_moveCount.ToString());
            
            tb.Add(moveCountLabel);

            var table = gm.UI.Stage.AddElement(tb) ;

        }

    }

    void UpdateStatus()
    {
        CheckGoal();
    }
    void CheckGoal()
    {
        if (Scene.TryFindComponent<Goal>(out Goal goal))
        {
            if (goal.Location == _controller.CurrentLocation)
            {
                Scene.GetOrCreateSceneComponent<GameManager>().GameStateMachine.ChangeState<GameState.Winning>();
                Console.WriteLine("Touch goal");
            }
        }
    }
    public Player SetMoveCount(int amount)
    {
        if (_moveCount == amount) return this;
        _moveCount = amount;
        
        if(_controller != null) _controller.EnableControlMove = _moveCount > 0;
        moveCountLabel?.SetText(_moveCount.ToString());
        return this;

    }
}
