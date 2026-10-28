using UnityEngine;
using System;

public class MonsterInstance : IDamageable
{
    public MonsterData Data { get; private set; }
    public int Level { get; private set; }
    public int CurrentHP { get; private set; }
    
    // Event untuk memberi tahu UI/Debugger saat status berubah
    public event Action Changed; 

    private float Scale => 1f + (Level - 1) * 0.1f;
    
    public int MaxHP => Mathf.RoundToInt(Data.BaseHP * Scale) + Level * 2;
    public int Attack => Mathf.RoundToInt(Data.BaseATK * Scale);
    public int ExpToNext => Level * 10;

    public MonsterInstance(MonsterData data, int startLevel)
    {
        Data = data;
        Level = startLevel;
        CurrentHP = MaxHP;
    }

    public void TakeDamage(int amount)
    {
        CurrentHP = Mathf.Max(0, CurrentHP - amount);
        Changed?.Invoke();
    }

    public void GainExp(int amount)
    {
        // Logika level up dan evolusi sederhana
        Level++;
        CurrentHP = MaxHP; // Heal saat level up
        Changed?.Invoke();
        
        Debug.Log($"{Data.DisplayName} naik ke Lv {Level}!");
        
        if (Data.EvolveLevel > 0 && Level >= Data.EvolveLevel && Data.EvolvesTo != null)
        {
            Data = Data.EvolvesTo;
            Debug.Log($"{Data.DisplayName} berevolusi menjadi {Data.DisplayName}!");
        }
    }
}