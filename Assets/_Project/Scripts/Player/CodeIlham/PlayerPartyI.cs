using System;
using UnityEngine;

public class PlayerPartyI
{
    public MonsterInstanceI Partner { get; private set; }
    public int Potions { get; private set; } = 3;
    
    public Transform PlayerTransform { get; private set; }

    // 🟢 PERBAIKAN: Event harus punya parameter int
    public event Action<int> PotionsChanged;

    public void SetPartner(MonsterInstanceI partner)
    {
        Partner = partner;
    }

    public void AddPotions(int amount)
    {
        Potions += amount;
        PotionsChanged?.Invoke(Potions); // 🟢 Kirim parameter
    }

    public void UsePotion()
    {
        if (Potions > 0 && Partner != null)
        {
            Potions--;
            int healed = Partner.Heal(Mathf.RoundToInt(Partner.MaxHP * 0.4f));
            PotionsChanged?.Invoke(Potions); // 🟢 Kirim parameter
        }
    }
    
    public void SetPotions(int amount)
    {
        Potions = Mathf.Max(0, amount);
        PotionsChanged?.Invoke(Potions); // 🟢 Kirim parameter
    }

    public void SetPlayerTransform(Transform transform)
    {
        PlayerTransform = transform;
    }

    public void Reset()
    {
        Partner = null;
        Potions = 3;
        PotionsChanged?.Invoke(Potions); // 🟢 Kirim parameter
    }
}