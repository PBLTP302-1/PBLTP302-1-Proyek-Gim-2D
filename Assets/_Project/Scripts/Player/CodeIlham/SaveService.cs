using System;
using System.IO;
using UnityEngine;

public class SaveService
{
    public const int CurrentVersion = 1;
    public SaveData PendingLoad { get; set; }

    private string SavePath => Path.Combine(
        Application.persistentDataPath,
        "save_0.json"
    );

    public bool Save(SaveData data)
    {
        try
        {
            data.savedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"[SaveService] Saved to {SavePath}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveService] Save failed: {e.Message}");
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
            Debug.LogError($"[SaveService] Load failed: {e.Message}");
            return null;
        }
    }

    public bool HasSave() => File.Exists(SavePath);
}