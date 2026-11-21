using System;
using System.IO;
using UnityEngine;

public class SaveService
{
    public const int CurrentVersion = 1;
    public SaveData PendingLoad { get; set; }

    private string SavePath => Path.Combine(Application.persistentDataPath, "save_0.json");

    public bool HasSave() => File.Exists(SavePath);

    public bool Save(SaveData data)
    {
        try
        {
            data.savedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Save gagal: {e.Message}");
            return false;
        }
    }

    public SaveData Load()
    {
        if (!HasSave()) return null;
        try
        {
            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Load gagal: {e.Message}");
            return null;
        }
    }

    public void Delete()
    {
        if (HasSave()) File.Delete(SavePath);
    }
}