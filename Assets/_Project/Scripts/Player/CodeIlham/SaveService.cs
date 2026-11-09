using System;
using System.IO;
using UnityEngine;

public class SaveService
{
    public const int CurrentVersion = 1;
    
    // Data ini dipakai WorldInstaller untuk teleport player saat Load Game
    public SaveData PendingLoad { get; set; }

    private string SavePath => Path.Combine(Application.persistentDataPath, "save_0.json");

    public bool Save(SaveData data)
    {
        try
        {
            data.savedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"[SaveService] Berhasil simpan ke: {SavePath}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveService] Gagal simpan: {e.Message}");
            return false;
        }
    }

    public SaveData Load()
    {
        if (!File.Exists(SavePath)) return null;

        try
        {
            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveService] Gagal load: {e.Message}");
            return null;
        }
    }

    public bool HasSave() => File.Exists(SavePath);
}