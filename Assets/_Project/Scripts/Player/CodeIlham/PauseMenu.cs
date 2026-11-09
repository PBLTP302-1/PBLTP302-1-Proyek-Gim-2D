using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] private GameObject pausePanel; // Panel utama (overlay + menu)

    [Header("Menu - Isi Manual di Inspector")]
    [SerializeField] private TextInput menu; // TextInput dengan Rows sudah diisi TextMeshPro

    // 🟢 Teks ditulis manual di Inspector via TextInput.Rows
    // Tidak perlu array di script - cukup drag TextMeshPro ke Rows di Inspector
    private List<string> optionLabels = new List<string> { "Lanjutkan", "Simpan sekarang", "Kembali ke judul" };

    private GameServicesI services;
    private bool isOpen = false;

    void Awake()
    {
        // 🟢 PASTIKAN PANEL NONAKTIF DI AWAL (ini yang mencegah overlay muncul otomatis)
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        else
        {
            Debug.LogError("[PauseMenu] pausePanel NULL! Drag panel pause ke Inspector.");
        }

        if (menu != null)
        {
            menu.gameObject.SetActive(false); // Menu juga nonaktif di awal
        }
    }

    public void Construct(GameServicesI s)
    {
        services = s;
        Debug.Log("[PauseMenu] Construct berhasil. Tekan Esc untuk membuka menu.");
    }

    void Update()
    {
        // 🟢 PAKAI INPUT SYSTEM
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            if (isOpen) Close();
            else TryOpen();
        }
    }

    private void TryOpen()
    {
        if (services == null) return;
        if (services.Dialogue != null && services.Dialogue.IsRunning) return;
        if (services.Battle != null && services.Battle.IsBattleActiveOrLoading) return;
        if (services.Input != null && services.Input.IsLocked) return;

        Open();
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;

        // 🟢 AKTIFKAN PANEL & MENU
        if (pausePanel != null) pausePanel.SetActive(true);
        if (menu != null)
        {
            menu.gameObject.SetActive(true);
            menu.Picked -= OnPicked;
            menu.Picked += OnPicked;
            
            // 🟢 Isi teks dari optionLabels (sesuai dengan yang ditulis manual di Inspector)
            // TextMeshPro di Inspector akan di-override dengan teks ini
            menu.Open(optionLabels.ToArray());
        }

        if (services != null && services.Input != null)
        {
            services.Input.Lock();
        }

        Debug.Log("[PauseMenu] Terbuka");
    }

    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;

        // 🟢 NONAKTIFKAN PANEL & MENU
        if (pausePanel != null) pausePanel.SetActive(false);
        if (menu != null) menu.gameObject.SetActive(false);

        if (services != null && services.Input != null)
        {
            services.Input.Unlock();
        }

        Debug.Log("[PauseMenu] Ditutup");
    }

    private void OnPicked(int index)
    {
        if (index < 0 || index >= optionLabels.Count) return;

        string choice = optionLabels[index];
        Debug.Log($"[PauseMenu] Dipilih: {choice}");

        switch (choice)
        {
            case "Lanjutkan":
                Close();
                break;
            case "Simpan sekarang":
                SaveNow();
                break;
            case "Kembali ke judul":
                BackToTitle();
                break;
        }
    }

    private void SaveNow()
    {
        if (services == null || services.Save == null)
        {
            Debug.LogError("[PauseMenu] SaveService NULL!");
            return;
        }

        Vector2 pos = Vector2.zero;
        if (services.Party != null && services.Party.PlayerTransform != null)
        {
            pos = services.Party.PlayerTransform.position;
        }

        bool saved = services.Save.Save(SaveMapper.Capture(services, pos));

        if (services.Toast != null)
        {
            services.Toast.Show(saved ? "Perjalanan tersimpan." : "Penyimpanan gagal.");
        }

        Close();
    }

    private void BackToTitle()
    {
        Close();

        if (services != null && services.Scenes != null)
        {
            services.Scenes.GoTo("Scn_Title");
        }
    }
}