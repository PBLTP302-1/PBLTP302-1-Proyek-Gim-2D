using System;
using System.Collections;
using UnityEngine;

public class BattleSystem
{
    public event Action Started;
    public event Action<string> MessageShown;
    public event Action ChoiceRequested;
    public event Action<MonsterInstance> Attacked;
    public event Action<MonsterInstance, int, bool, Element?> Hit;
    public event Action Missed;
    public event Action<MonsterInstance> Fainted;
    public event Action Victory;
    public event Action<MonsterData> LevelUp;
    public event Action<MonsterData, MonsterData> Evolving;
    public event Action<MonsterInstance> Evolved;
    public event Action<BattleResult> Ended;

    public MonsterInstance Player { get; private set; }
    public MonsterInstance Enemy  { get; private set; }
    public bool IsBoss { get; private set; }

    private PlayerParty party;
    private BattleSession session;
    private SceneFlow scenes;

    private BattleAction chosen;
    private BattleResult result = BattleResult.None;

    private const float secondsPerChar = 0.018f;
    private const float evolveSeconds = 2.4f;
    public event Action<MonsterInstance> Healed;
    public Element? LastHitElement { get; private set; }
    public bool LastHitCritical { get; private set; }


    public void Construct(PlayerParty party, BattleSession session, SceneFlow scenes)
    {
        this.party = party;
        this.session = session;
        this.scenes = scenes;
    }

    public void Begin()
    {
        Player = party.Partner;
        Enemy = session.Enemy;
        IsBoss = session.IsBoss;
        result = BattleResult.None;

        Debug.Log($"[BattleSystem] Player.Exp sebelum battle: {Player.Exp}");

        Run().WrapCoroutine();
    }

    public float DamageScale(MonsterInstance user)
    {
        return IsBoss && user == Enemy ? 0.8f : 1f;
    }

    public bool UsePotion() 
    { 
        if (party.UsePotion()) 
        { 
            Healed?.Invoke(Player);
            return true; 
        } 
        return false; 
    }

    public void ChooseAction(BattleAction action) => chosen = action;
    public void ForceRun() => result = BattleResult.Run;

    public IEnumerator Say(string text, float hold = 0.8f)
    {
        MessageShown?.Invoke(text);
        float wait = hold + text.Length * secondsPerChar;
        for (float t = 0f; t < wait; t += Time.deltaTime)
        {
            if (t > text.Length * secondsPerChar && MenuInput.Confirm) break;
            yield return null;
        }
    }

    public void RaiseAttacked(MonsterInstance m) => Attacked?.Invoke(m);
    public void RaiseHit(MonsterInstance m, int dmg, bool crit, Element? elem) 
    { 
        LastHitElement = elem;
        LastHitCritical = crit;
        Hit?.Invoke(m, dmg, crit, elem); 
    }
    public void RaiseMissed() => Missed?.Invoke();

    IEnumerator Run()
    {
        Started?.Invoke();
        yield return new WaitForSeconds(0.5f);
        yield return Say(IsBoss
            ? $"{Enemy.Data.DisplayName}, penguasa Gua Petir, muncul!"
            : $"{Enemy.Data.DisplayName} liar Lv{Enemy.Level} muncul!", 0.9f);
        yield return Say($"Maju, {Player.Data.DisplayName}!", 0.7f);

        while (result == BattleResult.None)
        {
            chosen = null;
            ChoiceRequested?.Invoke();
            yield return new WaitUntil(() => chosen != null);

            if (chosen is RunAction && IsBoss)
            {
                yield return chosen.Execute(this, Player, Enemy);
                continue;
            }

            BattleAction enemyAction = PickEnemyAction();
            bool playerFirst = chosen.Priority > 0
                || Player.Speed > Enemy.Speed
                || (Player.Speed == Enemy.Speed && UnityEngine.Random.value < 0.5f);

            if (playerFirst)
            {
                yield return Turn(chosen, Player, Enemy);
                if (result != BattleResult.None) break;
                yield return Turn(enemyAction, Enemy, Player);
            }
            else
            {
                yield return Turn(enemyAction, Enemy, Player);
                if (result != BattleResult.None) break;
                yield return Turn(chosen, Player, Enemy);
            }
            if (result != BattleResult.None) break;
        }

        if (result == BattleResult.Win)  yield return WinCoroutine();
        else if (result == BattleResult.Lose) yield return LoseCoroutine();

        Ended?.Invoke(result);
        yield return new WaitForSeconds(0.3f);
        scenes.EndBattle(result);
    }
    IEnumerator Turn(BattleAction action, MonsterInstance user, MonsterInstance target)
    {
        if (user.IsFainted) yield break;
        yield return action.Execute(this, user, target);
        CheckFaint();
    }

    void CheckFaint()
    {
        if (Enemy.IsFainted && result == BattleResult.None) result = BattleResult.Win;
        else if (Player.IsFainted && result == BattleResult.None) result = BattleResult.Lose;
    }

    BattleAction PickEnemyAction()
    {
        return UnityEngine.Random.value < 0.35f ? new SkillAction() : new AttackAction();
    }

    IEnumerator WinCoroutine()
    {
        Fainted?.Invoke(Enemy);
        yield return new WaitForSeconds(0.6f);
        yield return Say(IsBoss
            ? $"{Enemy.Data.DisplayName} kalah!"
            : $"{Enemy.Data.DisplayName} liar kalah!", 0.8f);
        Victory?.Invoke();

        int exp = (10 + Enemy.Level * 8) * (IsBoss ? 2 : 1);
        yield return Say($"{Player.Data.DisplayName} mendapat {exp} EXP.", 0.9f);

        foreach (var growth in Player.GainExp(exp))
        {
            if (growth.Type == GrowthType.LevelUp)
            {
                LevelUp?.Invoke(growth.From);
                yield return Say(growth.Message, 1.5f);
                continue;
            }
            yield return Say($"Apa? {growth.From.DisplayName} mulai bersinar...", 1.2f);
            Evolving?.Invoke(growth.From, growth.To);
            yield return new WaitForSeconds(evolveSeconds);
            Evolved?.Invoke(Player);
            yield return Say(growth.Message, 1.5f);
            yield return new WaitForSeconds(1.2f);
        }

        if (!IsBoss)
        {
            int healed = Player.Heal(Mathf.RoundToInt(Player.MaxHP * 0.25f));
            if (healed > 0)
                yield return Say($"{Player.Data.DisplayName} beristirahat sejenak dan pulih {healed} HP.", 0.8f);
        }

        yield return new WaitForSeconds(1.0f);
    }

    IEnumerator LoseCoroutine()
    {
        Fainted?.Invoke(Player);
        yield return new WaitForSeconds(0.6f);
        yield return Say($"{Player.Data.DisplayName} pingsan...", 0.9f);
        yield return new WaitForSeconds(1.0f);
    }
}

public static class CoroutineHelper
{
    private static MonoBehaviour runner;

    public static void WrapCoroutine(this IEnumerator coroutine)
    {
        if (runner == null)
        {
            var go = new GameObject("CoroutineRunner");
            runner = go.AddComponent<CoroutineRunner>();
            UnityEngine.Object.DontDestroyOnLoad(go);
        }
        runner.StartCoroutine(coroutine);
    }
}

public class CoroutineRunner : MonoBehaviour
{
    public static CoroutineRunner Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}