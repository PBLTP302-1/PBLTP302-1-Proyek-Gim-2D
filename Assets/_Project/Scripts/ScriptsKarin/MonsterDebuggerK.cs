using UnityEngine;

public class MonsterDebuggerK : MonoBehaviour
{
    // --- PENANDA: KODE ASLI DARI MODUL ---
    [SerializeField] private MonsterData species;
    [SerializeField] private int level = 3;

    private MonsterInstance monster;

    [ContextMenu("1. Buat Monster")]
    public void CreateMonster()
    {
        if (species == null)
        {
            Debug.LogWarning("Species MonsterData belum diisi di Inspector!");
            return;
        }

        monster = new MonsterInstance(species, level);
        Debug.Log($"Monster dibuat: {monster.Data.DisplayName} (Lv. {monster.Level}) - HP Max: {monster.MaxHP}");
    }

    [ContextMenu("2. Beri 50 Damage")]
    public void TestDamage()
    {
        if (monster == null) CreateMonster();
        monster.TakeDamage(50);
        Debug.Log($"HP terkini setelah terkena damage: {monster.CurrentHP}/{monster.MaxHP}");
    }
}