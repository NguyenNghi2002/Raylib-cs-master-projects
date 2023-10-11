using Engine;
using Engine.DefaultComponents.Render.Primitive;
using Engine.SceneManager;
using Engine.UI;
using Raylib_cs;
using System.Numerics;
using System.Reflection;
using System.Security.AccessControl;


#if false
var path = Path.Combine(Environment.CurrentDirectory, "piano-wav");
var note = 9;
var Oct = 0;
var oldfilenames = Directory.GetFiles(path);
for (int i = 0; i < 88; i++)
{
    if (note >= NoteGame.NoteAlphabet.Length)
    {
        Oct++;
        note = 0;
    }
    var newFileName = $"{NoteGame.NoteAlphabet[note]}{NoteGame.Octaves[Oct]}";
    Console.WriteLine(newFileName);

    File.Move(oldfil);


    ++note;
}
foreach (var n in )
{

} 
#endif




//new NoteGame().Run();
internal class NoteGame : Engine.Core
{
    public static readonly uint[] Octaves = {  4};
    public static readonly string[] NoteAlphabet = {"C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };
    public static  string[] loadedNoteIDs;
    public override void Intitialize()
    {
        base.Intitialize();
        AddManager(new ImguiEntityManager());

        loadedNoteIDs =  LoadNotes();
        Scene = new PlayScene();
        //Scene.Scaling = DesignScaling.Truncate; 
    }
    string[] LoadNotes()
    {
        List<string> notes = new();
        for (int i = 0; i < Octaves.Length; i++)
        {
            for (int j = 0; j < NoteAlphabet.Length; j++)
            {
                var alpha = NoteAlphabet[j];
                var octave = Octaves[i];
                var noteName = $"{alpha}{octave}";

                ContentManager.Load<rSound>(noteName, $"piano-mp3/{noteName}.mp3");
                notes.Add(noteName);
                Console.WriteLine($"loaded {noteName}");
                //Task.Delay(5000);

            }
        }

        return notes.ToArray();
        Console.WriteLine("finished load sound");
    }
}

internal class PlayScene : Scene
{
    public const int GameWidth = 720;
    public const int GameHeight = 1280 ;
    public const short LedgeLineNumber = 5;
    public const float LedgeLineSpacing = 24;
    public PlayScene() : base("Piano", GameWidth, GameHeight, RayUtils.GetColor("000814"), Color.BLACK)
    {}

    public override void OnBegined()
    {
        CreateEntity("dwa")
            .AddComponent(new CircleRenderer(20, Color.RED))
            .AddComponent(new FollowCursor())
            ;
        var ledgerLinesEn = CreateEntity("LedgerLines", new Vector2(GameWidth, GameHeight) / 2)
            .AddComponent(new CircleRenderer(2, Color.WHITE))
            .AddComponent<LedgerLines>()
            ;
        for (int i = 0; i < LedgeLineNumber; i++)
        {
            CreateEntity($"LedgerLine_{i + 1}", Vector2.UnitY * (i) * LedgeLineSpacing)
                .AddComponent(new RectangleRenderer(GameWidth - 10, 1, RayUtils.GetColor("8ecae6")))
                .Transform.SetParent(ledgerLinesEn.Transform, false)
                ;
        }

        CreateEntity("Flinger", ledgerLinesEn.Transform.Position2)
            .AddComponent(new Flinger("C4"))
            .AddComponent(new CircleRenderer(LedgeLineSpacing / 2f, Color.RED))
            .Transform.SetParent(ledgerLinesEn.Transform, false)
            ;

    }


}
internal class AudioSource : Component
{
    AudioStream audio;
    public AudioSource(rSound sound)
    {
        SetAudioStream(sound.Stream);
    }
    public AudioSource SetAudioStream(AudioStream audioStream)
    {
        audio = audioStream;
        return this;
    }
    public void Play()
    {
        Raylib.PlayAudioStream(audio);
    }
}
internal class LedgerLines : Component
{
    public Entity[] Lines => GetLinesEntities();
    

    Entity[] GetLinesEntities()
        =>Transform.Childs.Select(c => c.Entity).ToArray();
    public override void OnRemovedFromEntity()
    {
    }
}


