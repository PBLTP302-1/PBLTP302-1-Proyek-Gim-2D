using System.Collections;
using UnityEngine;

public class AttackAction : BattleAction
{
    public override int Priority => 0;
    public override string Name => "Serang";

    public override IEnumerator Execute(BattleSystem battle, MonsterInstanceI user, MonsterInstanceI target)
    {
        string who = (user == battle.Enemy && !battle.IsBoss ? "Lawan " : "") + user.Data.DisplayName;
        yield return battle.Say($"{who} menyerang!", 0.7f);
        
        battle.RaiseAttacked(user);
        yield return new WaitForSeconds(0.2f);
        
        if (Random.value < 0.06f)
        {
            battle.RaiseMissed();
            yield return battle.Say("Serangan meleset!", 0.8f);
            yield break;
        }

        int damage = DamageFormula.Calculate(user, target, 14, 1f * battle.DamageScale(user), out bool critical);
        target.TakeDamage(damage);
        battle.RaiseHit(target, damage, critical, null);
        yield return new WaitForSeconds(0.45f);
        
        if (critical) yield return battle.Say("Serangan kritis!", 0.7f);
    }
}