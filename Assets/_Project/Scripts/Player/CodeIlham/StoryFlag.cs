using System;
using System.Collections.Generic;
using UnityEngine;

public class StoryFlags
{
    // 🟢 KONSTANTA FLAG (Sesuai Modul P6)
    public const string IntroSeen = "intro_seen";           // Narasi pembuka sudah dilihat
    public const string HasStarter = "has_starter";         // Sudah pilih partner
    public const string GotGift = "got_gift";               // Sudah dapat hadiah dari Pak Dagang
    public const string BossDefeated = "boss_defeated";     // Boss Raijugar dikalahkan

    private HashSet<string> flags = new HashSet<string>();

    // 🟢 EVENT: Dipanggil saat ada flag yang berubah
    // Digunakan oleh GuardNpc, FlagToggle, dll.
    public event Action<string> Changed;

    // 🟢 SET: Tandai flag sebagai "sudah terjadi"
    public void Set(string key)
    {
        if (flags.Add(key)) // Hanya trigger event jika flag benar-benar baru
        {
            Debug.Log($"[StoryFlags] Flag di-set: {key}");
            Changed?.Invoke(key);
        }
    }

    //  HAS: Cek apakah flag sudah ada
    public bool Has(string key)
    {
        return flags.Contains(key);
    }

    // 🟢 LOAD: Muat flag dari save file (digunakan saat load game)
    public void Load(List<string> savedFlags)
    {
        flags.Clear();
        if (savedFlags != null)
        {
            foreach (string flag in savedFlags)
            {
                flags.Add(flag);
            }
        }
        Debug.Log($"[StoryFlags] {flags.Count} flag dimuat dari save file");
    }

    // 🟢 GET ALL: Untuk disimpan ke save file
    public List<string> GetAll()
    {
        return new List<string>(flags);
    }

    // 🟢 CLEAR: Hapus semua flag (untuk new game)
    public void Clear()
    {
        flags.Clear();
        Debug.Log("[StoryFlags] Semua flag dihapus");
    }
}