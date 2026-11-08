using System.Collections;
using UnityEngine;

/// <summary>
/// Aksi Ramuan: pulih 40% MaxHP, priority tinggi (1).
/// </summary>
public class ItemAction : BattleAction
{
    public override int Priority => 1;
    public override string Name => "Ramuan";

    public override IEnumerator Execute(BattleSystem battle,
        MonsterInstanceI user, MonsterInstanceI target)
    {
        battle.Party.UsePotion();

        int heal = Mathf.RoundToInt(user.MaxHP * 0.4f);
        int actual = user.Heal(heal);

        yield return battle.Say($"{user.Data.DisplayName} menggunakan Ramuan dan pulih {actual} HP!", 0.9f);
    }
}