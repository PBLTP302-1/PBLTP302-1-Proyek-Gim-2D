using UnityEngine;

public interface IDamageableP
{
    int CurrentHP { get; }
    int MaxHP { get; }
    void TakeDamage(int amount);
}