using System.Collections;
using UnityEngine;

public class SkillAction : BattleAction
{
    public override int Priority => 0;
    public override string Name => "Jurus";

    public override IEnumerator Execute(BattleSystem battle, MonsterInstanceI user, MonsterInstanceI target)
    {
        string who = (user == battle.Enemy && !battle.IsBoss ? "Lawan " : "") + user.Data.DisplayName;
        yield return battle.Say($"{who} memakai {user.Data.SkillName}!", 0.7f);
        
        battle.RaiseAttacked(user);
        yield return new WaitForSeconds(0.2f);
        
        float bonus = ElementBonus(user, target);
        int damage = DamageFormula.Calculate(user, target, 22, bonus * battle.DamageScale(user), out bool critical);
        target.TakeDamage(damage);
        battle.RaiseHit(target, damage, critical, user.Data.Element);
        yield return new WaitForSeconds(0.45f);
        
        if (critical) yield return battle.Say("Serangan kritis!", 0.7f);
        if (bonus > 1f) yield return battle.Say("Sangat efektif!", 0.7f);
        else if (bonus < 1f) yield return battle.Say("Kurang efektif...", 0.7f);
    }

    private float ElementBonus(MonsterInstanceI user, MonsterInstanceI target)
    {
        return ElementChart.Multiplier(user.Data.Element, target.Data.Element);
    }
}