using Raylib_cs;

new GameCore().Run();

public class GameCore : Engine.Core
{
    public override void Intitialize()
    {
        base.Intitialize();

        Scene = new PlayScene("Sample",720,1280,Color.DARKBLUE,Color.BLACK);

        // Managers.Add(new ImguiEntityManager());
    }
}
