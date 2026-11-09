using UnityEngine;

// --- PENANDA: KODE ASLI DARI MODUL ---
public interface IDamageableK
{
    int CurrentHP { get; }
    int MaxHP { get; }
    void TakeDamage(int amount);
}