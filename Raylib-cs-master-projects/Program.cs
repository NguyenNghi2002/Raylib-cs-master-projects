using rImage = Raylib_cs.Image;
using Img = Engine.UI.Image;
using static Raylib_cs.Raylib;
using System.Numerics;
using Engine.UI;
using Engine;
using System.Text;
using System.Threading.Tasks.Sources;

namespace Raylib_cs
{
    public static class Core
    {
        static TimerManager timerManager = new TimerManager();
        public static void Update()
        {
            timerManager.Update();
        }

        public static ITimer Schedule(float duration,bool repeat,object context,Action<ITimer> action)
        {
            return timerManager.Schedule(duration,repeat,context,action);
        }
    }
    public static class Program
    {
        public static Stage? stage;


        public static void Main(string[] args)
        {





            Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
            Raylib.InitWindow(780, 320, "layoutTable");
            ///WHY THIS WORKED
            ///WHY THIS WORKED
            ///WHY THIS WORKED
            var noiseImg = GenImageCellular(100, 100, 10);
            var noiseTxt = LoadTextureFromImage(noiseImg);
            var icon = Raylib.LoadTexture(@"C:\Raylib-cs-master\Logo\raylib-cs_64x64.png");
            var ico = Raylib.LoadTexture(@"C:\Users\nghin\Downloads\i.gif");

            Table table = null;
            Action DSAJDKSAJAKL = () =>
            {
                table = new Table()
                .SetFillParent(true)
                .Center()
                .Pad(20);
                ;
                var label = new Label("Raylib-cs");
                var slider = new Slider(0,20,0.2f,true,SliderStyle.CreateRaylib());
                var image = new Img(new TextureDrawable(noiseTxt));
                var button = new TextButton("Lmao", TextButtonStyle.CreateRaylib()).AddLeftMouseListener((bt) => Console.WriteLine(Raylib.GetTime()));
                var imgbutton = new ImageButton( ImageButtonStyle.CreateRaylib(30,30,noiseTxt)).AddLeftMouseListener((bt) => Console.WriteLine(Raylib.GetTime()));
                var progress = new ProgressBar(0,20,0.1f,false,ProgressBarStyle.Create(Color.GRAY,Color.WHITE));
                var userInput = new TextField("",TextFieldStyle.CreateRaylib()).SetMessageText("Name").SetPreferredWidth(200);
                var passInput = new TextField("",TextFieldStyle.CreateRaylib()).SetMessageText("Password");
                var logButton = new TextButton("Log in", TextButtonStyle.CreateRaylib());
                var checkboxSave = new CheckBox("Save Password", new CheckBoxStyle());
                var checkboxDonSaVe = new CheckBox("Dont Password", new CheckBoxStyle());

                ///WHY THIS AIN'T WORKED
                ///WHY THIS AIN'T WORKED
                ///WHY THIS AIN'T WORKED
                ///WHY THIS AIN'T WORKED
               


                var buttonGroup = new ButtonGroup(checkboxSave,checkboxDonSaVe);
                buttonGroup.Clear();

                var nameList = new Table();
                var window = new Window("Raylib",WindowStyle.CreateRaylib());
                window.SetResizable(true).PadTop(20);
                var dialog = new Dialog("dialog",WindowStyle.CreateRaylib());
                dialog.SetResizable(true).PadTop(20);
                dialog.AddText("Lmao").Pad(20);
                dialog.AddText("Item1");
                dialog.AddText("Item2");
                dialog.Row();
                dialog.Add(new Label("DSwaw"));
                dialog.Add(new Label("DSwaw"));
                dialog.Add(new Label("DSwaw"));
                dialog.Add(new Label("DSwaw"));
                dialog.Add(slider);
                dialog.AddText("Item4");
                dialog.AddText("Item5");
                dialog.AddText("Imte6");
                dialog.Row();
                dialog.AddText("Item7");
                dialog.AddText("Item8");
                dialog.AddText("Item9");

#if false
                nameList.Add(new Label("tablelayout"));
                nameList.Row();
                nameList.Add(new Label("Raylib_cs"));
                nameList.Add(new Img(new TextureDrawable(icon))).Pad(20);
                nameList.Row();
                nameList.Add(new Label("...")); 
#endif

#if false
                table.Add(new Label("Framework:"));
                table.Add(new Label("Raylib")).Pad(10);
                table.Add(nameList).Expand(); 
#endif

                Stack stack = new Stack();
                Table stackTable = new Table();
                stackTable.Add(new Label ("Hellow"));
                stackTable.Add(new Label ("World"));
                stackTable.Add(new Label ("Mother fucker"));

                var toolTip = new TextTooltip("lmao", slider, TextTooltipStyle.CreateRaylib());



                stack.Add(slider);
                stack.Add(stackTable);

                table.Add(stack);

                window.Add(label);

                table.Add(new Label("Layout table")).Size(100).Expand();
                table.Row();
                table.Add(userInput).Height(20).Pad(10);
                table.Row();
                table.Add(passInput).Height(20).Pad(10);
                table.Row();
                table.Add(checkboxSave).Size(200,30);
                table.Add(checkboxDonSaVe).Size(200,30);
                table.Row();
                table.Add(logButton).PrefSize(100,40);


                stage = new Stage();
                stage.AddElement(table);
                //stage.AddElement(dialog);
                stage.AddElement(toolTip);
                //stage
            };

            DSAJDKSAJAKL.Invoke();
            Camera2D camera2D = new Camera2D(Vector2.Zero, Vector2.Zero, 0, 1f);

            while (!Raylib.WindowShouldClose())
            {







                stage?.Update(GetMousePosition());
                Core.Update();
               
                if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_BRACKET))
                {
                    DSAJDKSAJAKL?.Invoke();
                }


                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.BLACK);

                stage?.Render(camera2D);

                if (Raylib.IsKeyPressed(KeyboardKey.KEY_ONE))
                {
                    stage.SetDebugTableUnderMouse( Table.TableDebug.Cell);
                }
                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();


        }
#if false
        public static void Main(string[] args)
        {
            Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
            Raylib.InitWindow(1280, 720, "ngu");

            bool completed = true;
            bool check = false;
            int value = 0;

            var a = Loader.Load<SpriteCollection>("sprite2.json");
            var sp = Loader.Load<Sprite>("sprite");

            var tilemap = new TiledMap(@"C:\Users\nghin\source\repos\MongameProject\MongameProject\bin\Debug\net6.0\Content\gold_map_01.tmx");
            tilemap.Position = new Vector2(200, 200);



            foreach (var item in collection)
            {

            }

            //GuiSetStyle((int)GuiControl.DEFAULT, (int)GuiDefaultProperty.TEXT_SIZE,32);
            GuiSetStyle((int)GuiControl.PROGRESSBAR, (int)GuiControlProperty.BORDER_COLOR_NORMAL, (uint)Raylib.ColorToInt(Color.RED));
            while (!Raylib.WindowShouldClose())
            {
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
                    tilemap.Reload();

                Raylib.BeginDrawing();

                Raylib.ClearBackground(Color.BLUE);
                tilemap.Draw();
                // Raylib.DrawTexturePro(sp.Texture,sp.Source,new Rectangle(50,50,50,50),Vector2.Zero,0,);

                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();
        } 
#endif
    }
}