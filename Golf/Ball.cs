using Engine;
using Engine.AI.FSM;
using Engine.SceneManager;
using Engine.Timer;
using Engine.UI;
using Engine.Velcro;
using Raylib_cs;
using System.Security;

namespace Golf
{
    public class GamePlaying : State<GameManager>
    {
        
        public override void Reason()
        {
            
        }
        public override void Update(float deltaTime)
        {
        }
    }
    public class GamePausing : State<GameManager>
    {
        public override void Update(float deltaTime)
        {
        }
        public override void Begin()
        {
        }
    }

    public class GameManager : SceneComponent
    {
        StateMachine<GameManager> StateMachine;
        State<GameManager>[] States = {
            new GamePlaying(),    
            new GamePausing(),    
        };


        UICanvas ui;

        Dialog retryDialog;

        Dialog WinDialog;

        Ball Ball;


        public override void OnAddedToScene()
        {
            StateMachine = new StateMachine<GameManager>(this, new GamePlaying());

            if(Scene.TryFindComponent<Ball>(out Ball))
            {
                
            }

            if(Scene.TryFindComponent(out ui))
            {
                #region Retry UI
                retryDialog = new Dialog("", WindowStyle.CreateRaylib());
                retryDialog.SetMovable(false);

                retryDialog.AddText(new Label("Out of jump", rFont.Default, Color.GRAY))
                    .PadTop(10);
                retryDialog.Row();

                retryDialog.Pad(10);
                var retryButton = retryDialog.AddButton("Retry", TextButtonStyle.CreateRaylib());
                retryButton.OnClicked += btt =>
                {

                    Ball.RemainJump = 1;
                    Ball.Entity.GetComponent<SlingShotBodyController>().Enable = true;
                    HideRetryMenu();
                };

                retryDialog.AddButton("Exit", TextButtonStyle.CreateRaylib());
                retryDialog.AddButton("Exit", TextButtonStyle.CreateRaylib());
                #endregion

                WinDialog = new Dialog("", WindowStyle.CreateRaylib());
                WinDialog.SetMovable(false);
                WinDialog.AddText("You win");
                WinDialog.AddButton("Next level", TextButtonStyle.CreateRaylib());
            }
        }


        public void HideRetryMenu()
        {
            retryDialog.Hide();

        }
        public void ShowRetryMenu()
        {
            ui.Stage.FindAllElementsOfType<Dialog>().ForEach(d => d.Hide());
            retryDialog.Show(ui.Stage);
            retryDialog.SetWidth(retryDialog.GetStage().GetWidth()); 
            retryDialog.SetPosition(retryDialog.GetStage().GetWidth()/2f - 250,0);
        }
        public void ShowWinMenu()
        {
            ui.Stage.FindAllElementsOfType<Dialog>().ForEach(d => d.Hide());
            WinDialog.Show(ui.Stage);
        }
        public void StartGame()
        {
            if (Ball.Entity.TryGetComponent<SlingShotBodyController>(out var slingShot))
            {
                slingShot.Enable = true;
            }
        }
        public void WinGame()
        {
            StopGame();
            ShowWinMenu();
        }
        public void StopGame()
        {
            if (Ball.Entity.TryGetComponent<SlingShotBodyController>(out var slingShot))
            {
                slingShot.Enable = false;
            }
        }

        public override void Update()
        {
            StateMachine.Update(Time.DeltaTime);
        }


    }

    public class Ball : Component,IUpdatable
    {
        SlingShotBodyController _slingShotBodyController;
        ITimer coolDownTimer;
        public float coolDownDuration = 1f;
        int _remainJump = 1;

        UICanvas _canvas;
        ProgressBar _cooldownProgressBar;
        Label _jumpCountLabel;
        
        public int UpdateOrder { get; set; }

        public override void OnAddedToEntity()
        {
            if(Scene.TryFind("uiCanvas",out var entity))
            {
                if(entity.TryGetComponent(out _canvas))
                {
                    var barStyle = SliderStyle.CreateRaylib();
                    _cooldownProgressBar = new ProgressBar(0, 1f, 0.01f, false, barStyle);
                    _cooldownProgressBar.SetMinMax(0f,1f);
                    _jumpCountLabel = new Label(_remainJump.ToString(),20);


                    var statusTable = new Table()
                        .SetFillParent(true)
                        .Center().Bottom()
                        .Pad(10)
                        //.DebugAll()
                        ; 

                    var textTable = new Table();
                    textTable.Add(new Label("Jump Remain: "));
                    textTable.Add(_jumpCountLabel);

                    statusTable.Add(textTable);
                    statusTable.Row();

                    var stack = new Engine.UI.Stack();
                    stack.Add(_cooldownProgressBar);
                    stack.Add(new Label("Cooldown",16))
                        .Center();

                    statusTable.Add(stack)
                        .SetExpandX()
                        .Size(500,20);
                    _canvas.Stage.AddElement(statusTable);
                    ;
                }

            }

#if true

            if (Entity.TryGetComponent(out _slingShotBodyController))
            {
                _slingShotBodyController.OnShooted += (controller) =>
                {
                    --RemainJump;
                    controller.Enable = false; //disable when cube is launched

                    coolDownTimer = Core.Schedule(coolDownDuration, false, this, (timer) =>
                    {
                        Ball ball = timer.Context as Ball;
                        if (_remainJump > 0)
                        {
                            controller.Enable = true;
                            Debugging.Log("jump count : " + _remainJump);


                        }
                        else
                        {
                            Debugging.Log("No more jump");
                            Scene.GetOrCreateSceneComponent<GameManager>()
                            .ShowRetryMenu();
                        }
                        ball.coolDownTimer = null;
                    });
                };
            } 
#endif
        }

        public int RemainJump
        {
            get => _remainJump;
            set => SetRemainJump(value);
        }
        public void SetRemainJump(int count)
        {
            _remainJump = count;
            _jumpCountLabel.SetText(_remainJump.ToString());
        }


        private float fillTime = 0;
        public void Update()
        {
            if(coolDownTimer != null)
            {
                _cooldownProgressBar.SetValue(1f- coolDownTimer.Elapse/coolDownDuration,true);
                fillTime = 0;
            }
            else
            {
                var duration = 1f;
                if(fillTime < 1)
                {
                    fillTime += Time.DeltaTime;
                    var fillVal = Easings.EaseElasticOut(fillTime, 0, 0.05f,duration);
                    //Console.WriteLine(fillVal);
                    _cooldownProgressBar.SetValue(fillVal,true);
                }

            }
        }

        public override void OnRemovedFromEntity()
        {
            coolDownTimer?.Stop();
            _slingShotBodyController = null;
            _canvas = null;
            _cooldownProgressBar = null;
        }
    }



}