using Engine;
using Engine.AI.FSM;
using Engine.SceneManager;
using Engine.UI;
using Raylib_cs;
using System.Numerics;
using System.Xml;

public static class GameState
{
    public class Pause : State<GameManager>
    {
        public override void OnInitialized()
        {
            var skin = Skin.DefaultSkin;


            var resumeBtt = _context.OptionDialog.AddButton("Resume", skin.Get<TextButtonStyle>());
            resumeBtt.Pad(10, 20, 10, 20);
            resumeBtt.OnClicked += btt =>
            {
                _machine.ChangeState<GameState.Resume>();
                (btt as IInputListener).OnMouseExit();
            };

            var retryBtt = _context.OptionDialog.AddButton("Retry", skin.Get<TextButtonStyle>());
            retryBtt.Pad(10, 20, 10, 20);
            retryBtt.OnClicked += btt =>
            {
                var transition = new Transition(() => new Gameplay((_context.Scene as Gameplay).Level));
                Core.StartTransition(transition);
            };

            var exitBtt = _context.OptionDialog.AddButton("Exit", skin.Get<TextButtonStyle>());
            exitBtt.OnClicked += btt => Core.StartTransition(new Transition(() => new MainMenu()));
            exitBtt.Pad(10,20,10,20);

        }
        public override void Begin()
        {
            _context.OptionDialog.Show(_context.UI.Stage);
        }
        public override void Reason()
        {
            var dialog = _context.OptionDialog;
            var dialogOrigin = new Vector2(dialog.GetX(), dialog.GetY());
            var dialogHitted = _context.OptionDialog.Hit(Input.MousePosition - dialogOrigin);
            if (Input.IsMouseReleased(MouseButton.MOUSE_BUTTON_LEFT) &&  dialogHitted == null )
            {
                if(!_context.OptionBtt.IsChecked)
                _context.OptionBtt.Toggle();
            }
        }
        public override void Update(float deltaTime)
        {
            
        }
        public override void End()
        {
            _context.OptionDialog.Hide();
        }


    }

    public class Winning : State<GameManager>
    {
        Dialog _dialog;
        public override void OnInitialized()
        {
            var skin = Skin.DefaultSkin;

            _dialog = new Dialog("", skin);
            _dialog.AddText("Win").Pad(20);
            var btt = _dialog.AddButton("Next level", skin.Get<TextButtonStyle>(), out Cell nextLevelCell);
            
            btt.OnClicked += btt =>
            {
                _dialog.Hide();
                var nextLevel = (_context.Scene as Gameplay).Level + 1;
                var path = @$"C:\Users\nghin\OneDrive\Desktop\Game Develop Tool\Tiled\Reundo\level{nextLevel}.tmx";
                Transition transition = File.Exists(path) ?
                    new FadeTransition(() => new Gameplay(nextLevel)) :
                    new FadeTransition(() => new MainMenu());
                Core.StartTransition(transition);
            };
        }
        public override void Begin()
        {
            _dialog.Show(_context.UI.Stage);
            _context.OptionBtt.SetTouchable(Touchable.Disabled);
        }
        public override void End()
        {
            _dialog.Hide();
        }

        public override void Update(float deltaTime)
        {
        }
    }


    public class Resume : State<GameManager>
    {
        public override void Begin()
        {
            _context.player._controller.EnableControlMove = _context.player.MoveCount > 0;
            _context.player._controller.EnableCommandMove = true;
        }
        public override void Update(float deltaTime)
        {
        }

        public override void End()
        {
            _context.player._controller.EnableControlMove = false;
            _context.player._controller.EnableCommandMove= false;

        }
    }
}
