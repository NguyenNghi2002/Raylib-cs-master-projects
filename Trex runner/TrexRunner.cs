using Engine;
using Raylib_cs;

namespace Trex_runner
{
    public class TrexRunner : Core
    {
        public override void Intitialize()
        {
            WindowWidth = 1280;
            WindowHeight = 300;
            base.Intitialize();
            Scene = new GameScene(); 
        }
    }
}