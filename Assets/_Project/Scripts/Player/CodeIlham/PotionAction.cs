using System.Collections;
using UnityEngine;

public class PotionAction : BattleAction
{
    public override int Priority => 1;
    public override string Name => "Ramuan";

    public override IEnumerator Execute(BattleSystem battle, MonsterInstanceI user, MonsterInstanceI target)
    {
        yield return battle.Say($"{user.Data.DisplayName} menggunakan Ramuan!", 0.7f);
        
        int healAmount = Mathf.RoundToInt(user.MaxHP * 0.4f);
        int actualHeal = user.Heal(healAmount);
        
        yield return battle.Say($"{user.Data.DisplayName} pulih {actualHeal} HP!", 0.8f);
        
        if (user == battle.Player) battle.Party.UsePotion();
    }
}