using UnityEngine;

public class MonsterDebugger : MonoBehaviour
{
    [SerializeField] private MonsterData species;
    [SerializeField] private int startLevel = 3;
    private MonsterInstance monster;

    void Start()
    {
        if (species != null)
        {
            monster = new MonsterInstance(species, startLevel);
            monster.Changed += OnMonsterChanged;
            Debug.Log($"Monster dibuat: {monster.Data.DisplayName} Lv {monster.Level}, HP: {monster.CurrentHP}/{monster.MaxHP}");
        }
    }

    private void OnMonsterChanged()
    {
        Debug.Log($"Status berubah: HP {monster.CurrentHP}/{monster.MaxHP}");
    }

    // Klik titik tiga di Inspector untuk memanggil ini
    [ContextMenu("1. Buat Monster")]
    void CreateMonster() => Start();

    [ContextMenu("2. Beri Damage 10")]
    void TakeDamage() => monster?.TakeDamage(10);

    [ContextMenu("3. Beri 100 EXP")]
    void GainExp() => monster?.GainExp(100);
}