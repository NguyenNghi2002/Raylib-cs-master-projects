using BBgame;
using Engine.TiledSharp;
using Engine.UI;
using ImGuiNET;
using Lidgren.Network;
using Raylib_cs;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Xml;
namespace Engine
{

    #region connect
    enum PacketType
    {
        Login
    }

    public class LoginInformation
    {
        public string Name;
    }

    class NetWorkConnection
    {
        private NetClient _client;

        public bool Start()
        {
            var loginInfo = new LoginInformation() { Name = "Random" };
            var config = new NetPeerConfiguration("networkGame");

            _client = new NetClient(config);
            _client.Start();
            var outMsg = _client.CreateMessage();
            outMsg.Write((byte)PacketType.Login);
            outMsg.WriteAllFields(loginInfo);
            _client.Connect("127.0.0.1", 9981, outMsg);
            return VerifyInfo();
        }

        private bool VerifyInfo()
        {
            var startTime = DateTime.Now;
            NetIncomingMessage inc = null;
            while (true)
            {
                if (DateTime.Now.Subtract(startTime).Seconds > 2)
                {
                    return false;
                }

                if ((inc = _client.ReadMessage()) == null) continue;

                switch (inc.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        var data = inc.ReadByte();
                        if (data == (byte)PacketType.Login)
                        {
                            var accepted = inc.ReadBoolean();
                            return accepted;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    } 
    #endregion
    public class LineRenderer2D
    {
        List<Segment> segments = new List<Segment>();
        List<Vector2> points = new List<Vector2>();
        Texture2D Texture2D;
        struct SegmentPoint
        {
            public Vector2 Position;
            public Vector4 Color;
            public float Width;
            public float Length;
        }
        class Segment
        {
            public Vector2 Tl,Tr,Bl,Br;
            public SegmentPoint Point;
            public SegmentPoint NextPoint;
            public Vector2 FusedPoint;
        }
        public LineRenderer2D()
        {
            points.Add(new Vector2());
            points.Add(new Vector2(20,0));
        }

        void UpdateAllSegments(ref List<Segment> segments)
        {
            segments.Clear();

            Vector2 normal = Vector2.Zero;
            for (int i = 0; i < points.Count() - 1; i++)
            {
                var seg = new Segment();

                var p0 = points[i];
                var p1 = points[i+1];

                //botleft

            }
        }

         static Vector2 Rot90CCW(Vector2 vector) => new Vector2(-vector.Y, vector.X);
        static Vector2 Rot90CW(Vector2 vector) => new Vector2(vector.Y, -vector.X);
    }
    internal class Program
    {
        static void Main(string[] args)
        {

            Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE | ConfigFlags.FLAG_VSYNC_HINT);
            Raylib.InitWindow(500, 400, "App");
            Stage s = new Stage();

            Table table = new Table()
                .Center()
                .DebugAll()
                .SetFillParent(true)
                ;
            table.Add(new Label("lmaodassdwadwa"));
            s.AddElement(table);
            
            while (!Raylib.WindowShouldClose())
            {
                s.Update();
                var m = Raylib.GetMousePosition();

                //trailSystem.UpdateTrail(Time.DeltaTime);
                Raylib.BeginDrawing();
                Raylib.ClearBackground( Color.BLACK);

                s.Render(default(Camera2D));

                Raylib.DrawFPS(10,10);
                RayUtils.DrawCircleLines(Raylib.GetMousePosition(), 20,3, Color.RED);
                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }
}