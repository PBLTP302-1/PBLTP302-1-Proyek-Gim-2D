using System;
using System.Collections.Generic;

public class StoryFlags
{
    // 🟢 KONSTANTA FLAG
    public const string IntroSeen = "intro_seen";
    public const string HasStarter = "has_starter";
    public const string BossDefeated = "boss_defeated";
    public const string GotGift = "got_gift";

    private HashSet<string> flags = new HashSet<string>();

    // 🟢 EVENT: Dipanggil saat flag berubah
    public event Action<string> Changed;

    // 🟢 SET: Tandai flag sebagai "sudah terjadi"
    public void Set(string key)
    {
        if (flags.Add(key)) 
        {
            Changed?.Invoke(key);
        }
    }

    // 🟢 HAS: Cek apakah flag sudah ada
    public bool Has(string key)
    {
        return flags.Contains(key);
    }

    // 🟢 LOAD: Muat flag dari save file
    public void Load(List<string> savedFlags)
    {
        flags.Clear();
        if (savedFlags != null)
        {
            foreach (string f in savedFlags) flags.Add(f);
        }
    }

    // 🟢 GET ALL: Untuk disimpan ke save file
    public List<string> GetAll()
    {
        return new List<string>(flags);
    }
}