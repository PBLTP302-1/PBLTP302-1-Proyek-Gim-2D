using UnityEngine;

public class MonsterDebuggerI : MonoBehaviour
{
    [SerializeField] private MonsterDataI species;
    [SerializeField] private int level = 4;
    private MonsterInstanceI instance;

    [ContextMenu("1. Buat monster")]
    private void Create() {
        instance = new MonsterInstanceI(species, level);
        Debug.Log($"Dibuat: {instance.Data.DisplayName} Lv {instance.Level}, HP {instance.CurrentHP}/{instance.MaxHP}");
    }

    [ContextMenu("2. Info")]    
    private void Info() {
        if (instance == null) { Debug.LogWarning("Belum dibuat!"); return; }
    Debug.Log($"{instance.Data.DisplayName} Lv {instance.Level}, " +
              $"HP {instance.CurrentHP}/{instance.MaxHP}, " +
              $"ATK {instance.Attack}, " +
              $"DEF {instance.Defense}, " +
              $"EXP to next: {instance.ExpToNext}");
    }

    [ContextMenu("3. Beri 100 EXP")]
    private void GiveExp() {
        if (instance == null) { Debug.LogWarning("Belum dibuat!"); return; }
        instance.GainExp(100);
    }
}
