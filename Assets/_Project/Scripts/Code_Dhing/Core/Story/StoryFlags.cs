using System;
using System.Collections.Generic;
using UnityEngine;

public class StoryFlags
{
    public const string IntroSeen = "intro_seen";
    public const string HasStarter = "has_starter";
    public const string BossDefeated = "boss_defeated";
    public const string MerchantGift = "merchant_gift";

    private HashSet<string> flags = new HashSet<string>();
    public event Action<string> Changed;

    public bool Has(string key) => flags.Contains(key);

    public void Set(string key)
    {
        if (flags.Add(key))
        {
            Debug.Log($"[StoryFlags] ✅ Flag SET: {key}");
            Changed?.Invoke(key);
        }
    }

    public void Clear(string key)
    {
        if (flags.Remove(key))
            Debug.Log($"[StoryFlags] ❌ Flag CLEAR: {key}");
    }

    public List<string> GetAll()
    {
        var list = new List<string>(flags);
        Debug.Log($"[StoryFlags] GetAll: {string.Join(", ", list)}");
        return list;
    }

    public void Load(List<string> list)
    {
        flags.Clear();
        if (list != null)
        {
            foreach (var f in list)
                flags.Add(f);
            Debug.Log($"[StoryFlags] Load {list.Count} flags: {string.Join(", ", list)}");
        }
        else
        {
            Debug.LogWarning("[StoryFlags] Load dengan list null!");
        }
    }
}