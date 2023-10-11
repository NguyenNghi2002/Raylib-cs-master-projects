public class SceneManager
{
    private static SceneManager _instance;
    public static SceneManager Instance => GetInstance();
    public static SceneManager GetInstance()
    {
        if (_instance == null)
            _instance = new SceneManager();
        return _instance;
    }

    internal static class States
    {
        internal static LoadingSceneState LoadingState        = new LoadingSceneState();
        internal static EnteringSceneState EnterState         = new EnteringSceneState();
        internal static ProcessingSceneState ProcessingState = new ProcessingSceneState();
        internal static ExitingSceneState ExitState           = new ExitingSceneState();
    }

    /// <summary>
    /// set <see langword="true"/> at start of <see cref="LoadAsyncScene(SceneTransition)"/><br/>
    /// set <see langword="false"/>  at the end of <see cref="LoadingSceneState.LoadAsync(SceneManager)"/>
    /// </summary>
    internal bool IsLoading;

    internal bool RequestedLoadAsync;
    internal Scene? RequestScene;
    internal Scene current;


    /// <summary>
    /// Transition will be null at the end of entering state
    /// </summary>
    internal SceneTransition? Transition;

    /// <summary>
    /// Set Transition to  <see langword="null"/> in 
    /// <see cref="ProcessingSceneState.OnStart(SceneManager)"/>
    /// </summary>
    internal void PopTransition()
        => Transition = null;
    /// <summary>
    /// Set 
    /// <code> <see cref="current"/> = <see cref="RequestScene"/> </code> 
    /// to  <see langword="null"/> in <see cref="LoadingSceneState.OnStart(SceneManager)"/>
    /// </summary>
    internal void PopRequest()
    {
        Console.WriteLine("Pop");
        if(RequestScene != null) RequestScene.State = current.State;
        current = RequestScene ?? current;
        RequestScene = null;
    }
    internal void SwitchState(Scene scene,SceneState sceneState)
    {
        scene.State?.OnEnd(this);
        scene.State = sceneState;
        scene.State.OnStart(this);
    }

    public void LoadScene(SceneTransition transition)
    {
        IsLoading = true;
        RequestedLoadAsync = false;
        var newScene = transition.NewSceneCallBack.Invoke();

        Transition = transition;
        RequestScene = newScene;
        SwitchState(RequestScene,new LoadingSceneState());
    }
    public void LoadAsyncScene(SceneTransition transition)
    {
        RequestedLoadAsync = true;
        var newScene = transition.NewSceneCallBack.Invoke();

        Transition = transition;
        if (current == null)
        {
            current = newScene;
            SwitchState(current,new LoadingSceneState());
        }
        else
        {
            RequestScene = newScene;
        }
            
    }



    public void Update()
    {
        current?.State?.OnProcessing(this);
    }
    public void Draw()
    {
        current?.State?.OnRendering(this);
    }
    public bool HasRequest(out Scene scene)
    {
        scene = RequestScene;
        return RequestScene != null;
    }

}
