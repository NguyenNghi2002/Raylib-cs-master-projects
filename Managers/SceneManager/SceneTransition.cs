public class SceneTransition 
{
    internal float timer;
    internal readonly Func<Scene> NewSceneCallBack;

    internal void Tick()
        => timer -= Raylib_cs.Raylib.GetFrameTime();

    public readonly float FadeInDuration ;
    public readonly float FadeOutDuration;

    /// <summary>
    /// Return true to stop intro 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual bool PlayIntro(Scene context,float timer) => true;

    /// <summary>
    /// Return true to stop outtro
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual bool PlayOutro(Scene context, float timer) => true;
    public virtual void PlayHold(Scene context, double elapseTime) { }

    public SceneTransition(Func<Scene> newSceneCallBack, float fadeInDuration, float fadeOutDuration)
    {
        NewSceneCallBack = newSceneCallBack;
        FadeInDuration = fadeInDuration;
        FadeOutDuration = fadeOutDuration;
    }
}
