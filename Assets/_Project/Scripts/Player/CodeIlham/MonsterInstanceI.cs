using System;
using System.Collections.Generic;
using UnityEngine;

public class MonsterInstanceI : IDamageable
{
    public MonsterDataI Data { get; private set; }
    public int Level { get; private set; }
    public int CurrentHP { get; private set; }
    public int CurrentEXP { get; private set; } // 🟢 EXP saat ini
    
    public event Action Changed;

    private float Scale => 1f + (Level - 1) * 0.1f;

    public int MaxHP => Mathf.RoundToInt(Data.BaseHP * Scale) + Level * 2;
    public int Attack => Mathf.RoundToInt(Data.BaseATK * Scale);
    public int Defense => Mathf.RoundToInt(Data.BaseDEF * Scale);
    public int Speed => Mathf.RoundToInt(Data.BaseSPD * Scale);
    public int ExpToNext => Level * 10; // 🟢 EXP untuk naik ke level berikutnya

    public MonsterInstanceI(MonsterDataI data, int level)
    {
        Data = data;
        Level = level;
        CurrentHP = MaxHP;
        CurrentEXP = 0; // 🟢 Mulai dari 0
    }

    public void TakeDamage(int amount)
    {
        CurrentHP = Mathf.Max(0, CurrentHP - amount);
        Changed?.Invoke();
    }

    public int Heal(int amount)
    {
        int oldHP = CurrentHP;
        CurrentHP = Mathf.Min(MaxHP, CurrentHP + amount);
        Changed?.Invoke();
        return CurrentHP - oldHP;
    }

    public void FullHeal()
    {
        CurrentHP = MaxHP;
        Changed?.Invoke();
    }

    // 🟢 METHOD GAIN EXP - WAJIB BENAR
    public GrowthEvent[] GainExp(int amount)
    {
        var events = new List<GrowthEvent>();
        CurrentEXP += amount; // 🟢 Tambah EXP

        Debug.Log($"[MonsterInstance] GainExp: +{amount}, CurrentEXP={CurrentEXP}, ExpToNext={ExpToNext}");

        while (CurrentEXP >= ExpToNext)
        {
            CurrentEXP -= ExpToNext; // 🟢 Kurangi EXP yang sudah dipakai
            Level++;

            MonsterDataI oldData = Data;
            bool evolved = false;

            if (Data.EvolveLevel > 0 && Level >= Data.EvolveLevel && Data.EvolvesTo != null)
            {
                Data = Data.EvolvesTo;
                evolved = true;
            }

            events.Add(new GrowthEvent
            {
                Type = evolved ? GrowthType.Evolution : GrowthType.LevelUp,
                From = oldData,
                To = Data,
                Message = evolved
                    ? $"{oldData.DisplayName} berevolusi menjadi {Data.DisplayName}!"
                    : $"{Data.DisplayName} naik ke Lv {Level}!"
            });

            Heal(Mathf.RoundToInt(MaxHP * 0.25f));
            Debug.Log($"[MonsterInstance] Level Up! Level={Level}, CurrentEXP={CurrentEXP}, ExpToNext={ExpToNext}");
        }

        Changed?.Invoke();
        return events.ToArray();
    }
}