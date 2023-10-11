using Raylib_cs;
using System.Diagnostics;


public class ProcessingSceneState : SceneState
{
    #region SceneBaseState

    public override void OnProcessing(SceneManager context)
    {
        // When there is request on loading scene
        // current scene will switch to exitting state 
        if (context.RequestScene != null)
            context.SwitchState(context.current,SceneManager.States.ExitState);
    }
    public override void OnRendering(SceneManager context)
    {
        base.OnRendering(context);
    }
    public override void OnStart(SceneManager context)
    {
        //TODO Unload previous scene here
        
        base.OnStart(context);
        context.PopTransition();
    }
    #endregion
}

public class EnteringSceneState : SceneState
{
    #region SceneBaseState

    public override void OnStart(SceneManager context)
    {
        //If transition is null then skip to processing state
        context.current.Start();
        context.PopRequest();

        base.OnStart(context);
        if (context.Transition == null)
        {
            context.SwitchState(context.current, SceneManager.States.ProcessingState);
        }
        else
        {
            context.Transition.timer = context.Transition.FadeInDuration;
        }
    }

    public override void OnRendering(SceneManager context)
    {
        //Todo : Fade IN transition
        context.Transition.Tick();
        if (context.Transition.PlayIntro(context.current,context.Transition.timer))
            context.SwitchState(context.current,SceneManager.States.ProcessingState);

        base.OnRendering(context);


        //int a = (byte)Raymath.Remap(timer, Duation, 0, 255, 0);
        //Raylib.DrawRectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight(), new Color(255, 255, 255, a));
        //base.OnRendering(context);
    }

    public override void OnEnd(SceneManager context)
    {
        base.OnEnd(context);
    }

    #endregion
}

public class ExitingSceneState : SceneState
{

    #region SceneBaseState

    public override void OnStart(SceneManager context)
    {

        if (context.Transition == null)
        {
            context.SwitchState(context.current,new LoadingSceneState());
        }
        else
        {
            context.Transition.timer = context.Transition.FadeOutDuration;
        }
        base.OnStart(context);
    }
    public override void OnRendering(SceneManager context)
    {
        context.Transition.Tick();
        if (context.Transition.PlayOutro(context.current,context.Transition.timer))
            context.SwitchState(context.current,SceneManager.States.LoadingState);

        //context.Transition.PlayOutro(context.current);
        //Todo : Fade out transition
        //int a = (byte)Raymath.Remap(timer, Duation, 0, 0, 255);
        //Raylib.DrawRectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight(), new Color(255, 255, 255, a));
        //base.OnRendering(context);

        base.OnRendering(context);

    }

    #endregion
}

public class LoadingSceneState : SceneState
{

    private void Load(Scene scene)
    {
        scene.LoadContent();
    }
    private async void LoadAsync(Scene scene)
    {
        //startTime = Raylib.GetTime();
        //bIsDone = false;
        await Task.Run(() =>
        {
            Load(scene);
          
        });
        //bIsDone = true;
    }

    #region SceneBaseState
    public override void OnStart(SceneManager context)
    {
        if (context.RequestedLoadAsync && context.RequestScene != null)
        {
            LoadAsync(context.RequestScene);
        }
        else
        {
            Load(context.current);
            context.SwitchState(context.current,SceneManager.States.EnterState);
        }
        base.OnStart(context);
    }
    public override void OnEnd(SceneManager context)
    {
        base.OnEnd(context);
    }
    public override void OnProcessing(SceneManager context)
    {
        base.OnProcessing(context);
        //if (bIsDone)
            context.SwitchState(context.current,SceneManager.States.EnterState);

    }
    public override void OnRendering(SceneManager context)
    {
        //if (bIsDone) return;

        //var elapseTime = Raylib.GetTime() - startTime;
        //context.Transition?.PlayHold(context.current, elapseTime); 

        Raylib.DrawText($"Loading {context.RequestScene}", 100, 100, 40, Color.WHITE);
        base.OnRendering(context);
    }
    #endregion






}
