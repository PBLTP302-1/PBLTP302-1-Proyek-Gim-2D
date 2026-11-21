using System.Collections;
using UnityEngine;

public abstract class BattleAction
{
    public abstract int Power { get; }
    public abstract int Priority { get; }
    public abstract IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target);

    protected virtual string MoveName(MonsterInstance user) => null;

    protected float ElementBonus(MonsterInstance user, MonsterInstance target)
    {
        return ElementChart.Multiplier(user.Data.Element, target.Data.Element);
    }
}

public class AttackAction : BattleAction
{
    public override int Power => 14;
    public override int Priority => 0;

    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        string who = (user == battle.Enemy && !battle.IsBoss ? "Lawan " : "") + user.Data.DisplayName;
        string move = MoveName(user);
        yield return battle.Say(move == null ? $"{who} menyerang!" : $"{who} memakai {move}!", 0.7f);
        battle.RaiseAttacked(user);
        yield return new WaitForSeconds(0.2f);

        if (Random.value < 0.06f)
        {
            battle.RaiseMissed();
            yield return battle.Say("Serangan meleset!", 0.8f);
            yield break;
        }

        float bonus = ElementBonus(user, target);
        int damage = DamageFormula.Calculate(user, target, Power, bonus * battle.DamageScale(user), out bool critical);
        target.TakeDamage(damage);
        battle.RaiseHit(target, damage, critical, move != null ? user.Data.Element : (Element?)null);
        yield return new WaitForSeconds(0.45f);

        if (critical) yield return battle.Say("Serangan kritis!", 0.7f);
        if (bonus > 1f) yield return battle.Say("Sangat efektif!", 0.7f);
        else if (bonus < 1f) yield return battle.Say("Kurang efektif...", 0.7f);
    }
}

public class SkillAction : BattleAction
{
    public override int Power => 22;
    public override int Priority => 0;
    protected override string MoveName(MonsterInstance user) => user.Data.SkillName;

    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        string who = (user == battle.Enemy && !battle.IsBoss ? "Lawan " : "") + user.Data.DisplayName;
        yield return battle.Say($"{who} memakai {user.Data.SkillName}!", 0.7f);
        battle.RaiseAttacked(user);
        yield return new WaitForSeconds(0.2f);

        float bonus = ElementBonus(user, target);
        int damage = DamageFormula.Calculate(user, target, Power, bonus * battle.DamageScale(user), out bool critical);
        target.TakeDamage(damage);
        battle.RaiseHit(target, damage, critical, user.Data.Element);
        yield return new WaitForSeconds(0.45f);

        if (critical) yield return battle.Say("Serangan kritis!", 0.7f);
        if (bonus > 1f) yield return battle.Say("Sangat efektif!", 0.7f);
        else if (bonus < 1f) yield return battle.Say("Kurang efektif...", 0.7f);
    }
}

public class PotionAction : BattleAction
{
    public override int Power => 0;
    public override int Priority => 1;

    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        if (!battle.UsePotion())
        {
            yield return battle.Say("Ramuan habis!", 0.7f);
            yield break;
        }
        yield return battle.Say($"{user.Data.DisplayName} minum Ramuan. HP pulih!", 0.8f);
    }
}

public class RunAction : BattleAction
{
    public override int Power => 0;
    public override int Priority => 2;

    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        if (battle.IsBoss)
        {
            yield return battle.Say("Tidak bisa kabur dari boss!", 0.8f);
            yield break;
        }
        if (Random.value < 0.7f)
        {
            yield return battle.Say("Berhasil kabur!", 0.7f);
            battle.ForceRun();
        }
        else
        {
            yield return battle.Say("Gagal kabur!", 0.7f);
        }
    }
}