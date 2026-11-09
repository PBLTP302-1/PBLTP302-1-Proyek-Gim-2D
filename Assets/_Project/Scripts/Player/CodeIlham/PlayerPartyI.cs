using System.Collections.Generic;
using UnityEngine;

public class PlayerPartyI
{
    public MonsterInstanceI Partner { get; private set; }
    public int Potions { get; private set; } = 3;
    
    // 🟢 TAMBAHKAN INI
    public Transform PlayerTransform { get; private set; }

    public void SetPartner(MonsterInstanceI partner)
    {
        Partner = partner;
    }

    public void AddPotions(int amount)
    {
        Potions += amount;
        PotionsChanged?.Invoke();
    }

    public void UsePotion()
    {
        if (Potions > 0 && Partner != null)
        {
            Potions--;
            int healed = Partner.Heal(Mathf.RoundToInt(Partner.MaxHP * 0.4f));
            PotionsChanged?.Invoke();
        }
    }
    public void SetPotions(int amount)
{
    Potions = Mathf.Max(0, amount);
    PotionsChanged?.Invoke();
}
    

    // 🟢 TAMBAHKAN METHOD INI
    public void SetPlayerTransform(Transform transform)
    {
        PlayerTransform = transform;
    }

    public void Reset()
    {
        Partner = null;
        Potions = 3;
    }

    public event System.Action PotionsChanged;
}