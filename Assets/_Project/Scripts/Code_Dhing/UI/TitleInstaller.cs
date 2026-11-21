using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleInstaller : SceneInstaller
{
    [SerializeField] private TextMenu menu;
    [SerializeField] private AudioClip titleMusic;

    private GameServices services;
    private List<string> options = new List<string>();

    public override void Install(GameServices s)
    {
        services = s;
        options.Clear();
        
        // Cek apakah ada file save
        if (s.Save.HasSave()) options.Add("Lanjutkan");
        options.Add("Mulai baru");
        options.Add("Keluar");

        menu.Picked += OnPicked;
        menu.Open(options.ToArray());

        // Putar musik judul
        if (s.Audio != null) s.Audio.PlayWorldMusic(titleMusic);
    }

    private void OnPicked(int index)
    {
        menu.Picked -= OnPicked;        
        string choice = options[index];
        if (choice == "Lanjutkan") Continue();
        else if (choice == "Mulai baru") NewGame();
        else if (choice == "Keluar") Quit();
    }

    private void Continue()
    {
        SaveData data = services.Save.Load();
        if (data == null) { NewGame(); return; }
        
        SaveMapper.Apply(data, services);
        services.Save.PendingLoad = data;
        services.Scenes.GoTo(SceneFlow.WorldScene);
    }

    private void NewGame()
    {
        services.Party.Reset();
        services.Flags.Load(new List<string>());
        services.Scenes.GoTo(SceneFlow.WorldScene);
    }

    private void Quit()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}