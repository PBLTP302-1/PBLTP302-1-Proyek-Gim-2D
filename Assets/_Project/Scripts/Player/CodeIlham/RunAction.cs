using System.Collections;
using UnityEngine;

public class RunAction : BattleAction
{
    public override int Priority => 2;
    
    // 🟢 TAMBAHKAN INI - Sesuai modul P5
    public override string Name => "Kabur";

    public override IEnumerator Execute(BattleSystem battle, MonsterInstanceI user, MonsterInstanceI target)
    {
        if (battle.IsBoss)
        {
            yield return battle.Say("Tidak bisa kabur dari pertarungan Boss!", 0.8f);
            yield break;
        }

        // 70% kemungkinan berhasil kabur
        if (UnityEngine.Random.value < 0.7f)
        {
            yield return battle.Say("Berhasil kabur dengan selamat!", 0.8f);
            battle.SetFled();
        }
        else
        {
            yield return battle.Say("Gagal kabur!", 0.8f);
        }
    }
}