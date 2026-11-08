using UnityEngine;

public class BattleSessionI
{
    public MonsterInstanceI Enemy { get; private set; }
    public bool IsBoss { get; private set; }
    
    private WildMonsterI currentWildMonster;
    private float playerSafeUntil;
    private bool battleActive = false;
    private bool battleLoading = false;

    // 🟢 WAKTU AMAN 4 DETIK
    public const float SafeSeconds = 4f;

    public void SetEnemy(MonsterInstanceI enemy, bool isBoss, WildMonsterI wildMonster)
    {
        Enemy = enemy;
        IsBoss = isBoss;
        currentWildMonster = wildMonster;
        battleActive = true;
        battleLoading = true;
    }

    public void SetBattleLoading(bool loading)
    {
        battleLoading = loading;
        if (loading) battleActive = true;
    }

    public void EndBattle(BattleResultI result)
    {
        battleActive = false;
        battleLoading = false;
        
        // 🟢 SET WAKTU AMAN 4 DETIK UNTUK SEMUA MONSTER
        if (result == BattleResultI.Win || result == BattleResultI.Flee)
        {
            playerSafeUntil = Time.time + SafeSeconds;
        }

        if (currentWildMonster != null)
        {
            currentWildMonster.OnBattleEnd(result);
        }
        
        currentWildMonster = null;
        Enemy = null;
        IsBoss = false;
    }

    public bool IsPlayerSafe()
    {
        return Time.time < playerSafeUntil;
    }

    public bool IsBattleActiveOrLoading => battleActive || battleLoading;
    public bool IsBattleActive => battleActive;
}