using System;
using UnityEngine;

public class PlayerParty
{
    public const int StartingPotions = 3;
    private int potions = StartingPotions;
    public float Playtime { get; private set; }
    public void AddPlaytime(float seconds) => Playtime += seconds;

    public MonsterInstance Partner { get; private set; }

    public int Potions
    {
        get => potions;
        set
        {
            if (potions != value)
            {
                potions = Math.Max(0, value);
                PotionsChanged?.Invoke();
            }
        }
    }

    public event Action PotionsChanged;

    public event Action PartnerChanged;
    public void SetPartner(MonsterInstance monster)
    {
        Partner = monster;
        PartnerChanged?.Invoke();
    }

    public bool UsePotion()
    {
        if (Potions <= 0 || Partner == null) return false;

        Potions--;
        Partner.Heal(Mathf.RoundToInt(Partner.MaxHP * 0.4f)); 
        
        PotionsChanged?.Invoke(); 
        return true;
    }

    public void Reset()
    {
        Partner = null;
        Potions = StartingPotions;
    }
}