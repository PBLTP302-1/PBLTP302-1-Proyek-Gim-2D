using System.Collections.Generic;
using UnityEngine;

public class TitleInstaller : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextInput menu;

    private GameServicesI services;
    private List<string> options = new List<string>();

    void Start()
    {
        services = GameBootstrapI.CurrentServices;
        
        if (services != null)
        {
            Debug.Log("[TitleInstaller] Services ditemukan via static reference");
            Install();
        }
        else
        {
            Debug.LogError("[TitleInstaller] GameBootstrapI.CurrentServices NULL!");
        }
    }

    private void Install()
    {
        if (menu == null)
        {
            Debug.LogError("[TitleInstaller] Menu NULL! Drag TitleMenu ke field Menu.");
            return;
        }

        options.Clear();

        if (services.Save != null && services.Save.HasSave())
        {
            options.Add("Lanjutkan");
            Debug.Log("[TitleInstaller] Save file ditemukan");
        }

        options.Add("Mulai baru");
        options.Add("Keluar");

        Debug.Log($"[TitleInstaller] Install dengan {options.Count} opsi");

        menu.Picked -= OnPicked;
        menu.Picked += OnPicked;
        menu.Open(options.ToArray());
    }

    private void OnPicked(int index)
    {
        if (index < 0 || index >= options.Count) return;

        string choice = options[index];
        Debug.Log($"[TitleInstaller] Dipilih: {choice}");

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

    // 🟢 RESET KOMPREHENSIF - Dipanggil sebelum pindah ke WorldScene
   private void ResetGameState()
    {
        Debug.Log("[TitleInstaller] === MEMULAI RESET STATE ===");

        // 1. Reset DialogueRunner (PALING PENTING!)
        if (services.Dialogue != null)
        {
            services.Dialogue.ForceStop(); // 🟢 Gunakan method baru ini
        }
        else
        {
            Debug.LogWarning("[TitleInstaller] ⚠ DialogueRunner NULL!");
        }

        // 2. Reset BattleSession (jika ada battle yang tersisa)
        if (services.Battle != null)
        {
            services.Battle.SetBattleLoading(false);
            Debug.Log("[TitleInstaller] ✓ BattleSession di-reset");
        }

        Debug.Log("[TitleInstaller] === RESET SELESAI ===");
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

        // 🟢 RESET STATE DULU SEBELUM APPLY
        ResetGameState();

        // Terapkan data save
        SaveMapper.Apply(data, services);
        services.Save.PendingLoad = data;

        Debug.Log($"[TitleInstaller] Load game: posisi {data.playerPosition}, partner {data.partnerId}");
        Debug.Log($"[TitleInstaller] Flags setelah load: {string.Join(", ", services.Flags.GetAll())}");

        // Pindah scene
        services.Scenes.GoTo(SceneFlowI.WorldScene);
    }

    private void NewGame()
    {
        // 🟢 RESET STATE DULU
        ResetGameState();

        // Reset Party (ramuan = 3, tanpa partner)
        if (services.Party != null)
        {
            services.Party.Reset();
            Debug.Log("[TitleInstaller] ✓ PlayerParty di-reset");
        }

        // Clear semua flag (agar intro bisa diputar ulang)
        if (services.Flags != null)
        {
            services.Flags.Clear();
            Debug.Log("[TitleInstaller] ✓ StoryFlags di-clear");
        }

        // Hapus pending load (jika ada)
        if (services.Save != null)
        {
            services.Save.PendingLoad = null;
        }

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