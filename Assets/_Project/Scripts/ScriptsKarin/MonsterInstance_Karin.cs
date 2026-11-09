using System;
using UnityEngine;

public class MonsterInstance_Karin : IDamageable
{
    // --- PENANDA: KODE ASLI DARI MODUL ---
    public MonsterData Data { get; private set; }
    public int Level { get; private set; }
    public int CurrentHP { get; private set; }
    
    public event Action Changed;

    private float Scale => 1f + (Level - 1) * 0.1f;
    public int MaxHP => Mathf.RoundToInt(Data.BaseHP * Scale) + Level * 2;
    public int Attack => Mathf.RoundToInt(Data.BaseATK * Scale);
    public int ExpToNext => Level * 10;

    public MonsterInstance_Karin(MonsterData data, int level)
    {
        Data = data;
        Level = level;
        CurrentHP = MaxHP;
    }

    public void TakeDamage(int amount)
    {
        CurrentHP = Mathf.Max(0, CurrentHP - amount);
        Changed?.Invoke();
    }
}