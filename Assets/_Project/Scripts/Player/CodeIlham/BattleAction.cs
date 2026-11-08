using System.Collections;

public abstract class BattleAction
{
    public abstract int Priority { get; }
    public abstract string Name { get; }
    public abstract IEnumerator Execute(BattleSystem battle, MonsterInstanceI user, MonsterInstanceI target);
}