using Engine;
using Engine.UI;
using Raylib_cs;
using System.Numerics;

internal class Flinger : Component,IUpdatable
{
    LedgerLines? ledgerLines => Entity.Transform.Parent.Entity.GetComponent<LedgerLines>();
    AudioSource audioSource;
    int IUpdatable.UpdateOrder { get; set; } = 0;

    int iterator = 0;
    public Flinger(string startNoteName)
    {
        iterator = Array.IndexOf(NoteGame.loadedNoteIDs, startNoteName);
    }
    public override void OnAddedToEntity()
    {
        audioSource = Entity.AddComponent(new AudioSource(ContentManager.Get<rSound>(NoteGame.loadedNoteIDs[iterator])));
        UpdatePoint();
    }

    void IUpdatable.Update()
    {
        if (Input.IsKeyPressed(KeyboardKey.KEY_UP))
        {
            iterator--; 
            UpdatePoint();
        }
        if (Input.IsKeyPressed(KeyboardKey.KEY_DOWN))
        {
            iterator++;
            UpdatePoint();
        }
    }
    void UpdatePoint()
    {
        if (iterator >=0 && iterator < NoteGame.loadedNoteIDs.Length &&
            ContentManager.TryGet(NoteGame.loadedNoteIDs[iterator],out rSound sound))
        {
            Console.WriteLine(NoteGame.loadedNoteIDs[iterator]);
            audioSource.SetAudioStream(sound.Stream);
        }
        audioSource?.Play();
        Transform.LocalPosition = new Vector3(0,iterator * PlayScene.LedgeLineSpacing/2f,0);
    }

}


