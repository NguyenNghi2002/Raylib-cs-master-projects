# Raylib-cs-master-projects
Custom Engine built on top of Raylib

Raylib Github: https://github.com/raysan5/raylib  <br/>
Raylib Website: https://www.raylib.com

## Example Application

1) Create an Application Core class inherited from ```csharp Engine.Core``` class
```csharp
public class GameCore : Engine.Core
{
    public const int MAX_FPS = 60; // Set cap fps to 60

    VectorInt2 Samsungs22PlusRes = new VectorInt2(1080, 2340); // samsung22S resolution
    VectorInt2 Window = new VectorInt2(1920/2, 1080/2); // window resolution

    
    public override void Initialize()
    {
        //                                 /*  WINDOW CONFIGURATION */
        ///--------------------------------------------------------------------------------///
        var windowHints = ConfigFlags.FLAG_WINDOW_RESIZABLE;
        Raylib.SetConfigFlags(windowHints);

        Raylib.SetTargetFPS(MAX_FPS);

        var currDecive = Window;

        WindowWidth = currDecive.Y;
        WindowHeight = currDecive.X;
        

        //                               /*  START WINDOW */
        ///---------------------------------------------------------------------------------///

        base.Intitialize();

        //                                 /*  CREATE SCENE */
        ///----------------------------------------------------------------------------------///
        var ratio = 1280f / 720f;
        Scene = new SampleScene("Sample", 720, 1280);
        //Scene.Scaling = Engine.SceneManager.DesignScaling.Truncate;


        //                                  /*  GLOBAL MANAGER */
        ///----------------------------------------------------------------------------------///
        Managers.Add(new ImguiEntityManager());
    }
}
```



# Engine Todos list:
- [ ] Give it a name
- [ ] Create a fully released game with the engine
- [ ] Create shader systems
- [ ] Update trail system
- [ ] Update particle system
