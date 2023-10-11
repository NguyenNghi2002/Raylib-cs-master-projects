public interface IState<Tcontext>
{
    public void OnStart(Tcontext context);
    public void OnProcessing(Tcontext context);
    public void OnEnd(Tcontext context);
}
