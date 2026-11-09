using System.Collections.Generic;
using UnityEngine;

public class TitleInstaller : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextInput menu;

    [Header("Audio")]
    [SerializeField] private AudioClip titleMusic; // 🟢 BARU

    private GameServicesI services;
    private List<string> options = new List<string>();

    void Start()
    {
        services = GameBootstrapI.CurrentServices;
        
        if (services != null)
        {
            Install();
        }
        else
        {
            Debug.LogError("[TitleInstaller] CurrentServices NULL!");
        }
    }

    private void Install()
    {
        if (menu == null)
        {
            Debug.LogError("[TitleInstaller] Menu NULL!");
            return;
        }

        options.Clear();

        if (services.Save != null && services.Save.HasSave())
        {
            options.Add("Lanjutkan");
        }

        options.Add("Mulai baru");
        options.Add("Keluar");

        menu.Picked -= OnPicked;
        menu.Picked += OnPicked;
        menu.Open(options.ToArray());

        // 🟢 PUTAR MUSIK JUDUL
        if (services.Audio != null && titleMusic != null)
        {
            services.Audio.PlayWorldMusic(titleMusic);
            Debug.Log($"[TitleInstaller] Memutar musik judul: {titleMusic.name}");
        }
    }

    private void OnPicked(int index)
    {
        if (index < 0 || index >= options.Count) return;

        string choice = options[index];

        switch (choice)
        {
            case "Lanjutkan":
                Continue();
                break;
            case "Mulai baru":
                NewGame();
                break;
            case "Keluar":
                Quit();
                break;
        }
    }

    private void Continue()
    {
        if (services.Save == null)
        {
            Debug.LogError("[TitleInstaller] SaveService NULL!");
            NewGame();
            return;
        }

        SaveData data = services.Save.Load();
        if (data == null)
        {
            Debug.LogWarning("[TitleInstaller] File save tidak valid, mulai baru.");
            NewGame();
            return;
        }

        SaveMapper.Apply(data, services);
        services.Save.PendingLoad = data;

        Debug.Log("[TitleInstaller] Load game berhasil");
        services.Scenes.GoTo(SceneFlowI.WorldScene);
    }

    private void NewGame()
    {
        if (services.Party != null) services.Party.Reset();
        if (services.Flags != null) services.Flags.Clear();

        Debug.Log("[TitleInstaller] Mulai game baru");
        services.Scenes.GoTo(SceneFlowI.WorldScene);
    }

    private void Quit()
    {
        Debug.Log("[TitleInstaller] Keluar game");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    void OnDestroy()
    {
        if (menu != null) menu.Picked -= OnPicked;
    }
}