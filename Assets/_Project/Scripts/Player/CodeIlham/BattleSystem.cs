using System;
using System.Collections;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private float secondsPerChar = 0.018f;

    public MonsterInstanceI Player { get; private set; }
    public MonsterInstanceI Enemy { get; private set; }
    public PlayerPartyI Party { get; private set; }
    public bool IsBoss { get; private set; }

    public BattleAction ChosenAction { get; set; }
    public bool HasFled { get; private set; }

    // Events
    public event Action Started;
    public event Action<string> MessageShown;
    public event Action ChoiceRequested;
    public event Action<MonsterInstanceI> Attacked;
    public event Action<MonsterInstanceI, int, bool, Element?> Hit;
    public event Action Missed;
    public event Action<MonsterInstanceI> Fainted;
    public event Action<MonsterDataI> LevelUp;
    public event Action<MonsterDataI, MonsterDataI> Evolving;
    public event Action<MonsterInstanceI> Evolved;

    private BattleResultI result = BattleResultI.None;
    private SceneFlowI scenes;
    private BattleSessionI session;
    private Coroutine battleCoroutine;
    private bool battleEnded = false; // 🟢 BARU: Flag untuk hentikan battle

    public void Construct(PlayerPartyI party, BattleSessionI session, SceneFlowI scenes)
    {
        Party = party;
        Player = party.Partner;
        Enemy = session.Enemy;
        IsBoss = session.IsBoss;
        this.session = session;
        this.scenes = scenes;
        battleEnded = false;
    }

    public void Begin()
    {
        if (battleCoroutine != null) StopCoroutine(battleCoroutine);
        battleCoroutine = StartCoroutine(Run());
    }

    // 🟢 BARU: Method untuk menghentikan battle
    public void EndBattle()
    {
        battleEnded = true;
        if (battleCoroutine != null)
        {
            StopCoroutine(battleCoroutine);
            battleCoroutine = null;
        }
        Debug.Log("[BattleSystem] Battle dihentikan");
    }

    private IEnumerator Run()
    {
        Started?.Invoke();
    yield return new WaitForSeconds(0.5f);

    yield return Say(IsBoss
        ? $"{Enemy.Data.DisplayName}, penguasa Gua Petir, muncul!"
        : $"{Enemy.Data.DisplayName} liar Lv{Enemy.Level} muncul!", 0.9f);
    yield return Say($"Maju, {Player.Data.DisplayName}!", 0.7f);

    while (result == BattleResultI.None && !battleEnded)
    {
        ChosenAction = null;
        HasFled = false;
        ChoiceRequested?.Invoke();
        yield return new WaitUntil(() => ChosenAction != null || battleEnded);

        if (battleEnded) yield break;

        if (ChosenAction is RunAction)
        {
            yield return Turn(ChosenAction, Player, Enemy);
            if (HasFled)
            {
                result = BattleResultI.Flee;
                break;
            }
            continue;
        }

        BattleAction enemyAction = PickEnemyAction();
        bool playerFirst = ChosenAction.Priority > 0
            || Player.Speed > Enemy.Speed
            || (Player.Speed == Enemy.Speed && UnityEngine.Random.value < 0.5f);

        if (playerFirst)
        {
            yield return Turn(ChosenAction, Player, Enemy);
            if (Enemy.CurrentHP <= 0) { yield return Win(); break; }
            yield return Turn(enemyAction, Enemy, Player);
        }
        else
        {
            yield return Turn(enemyAction, Enemy, Player);
            if (Player.CurrentHP <= 0) { yield return Lose(); break; }
            yield return Turn(ChosenAction, Player, Enemy);
        }

        if (Player.CurrentHP <= 0) yield return Lose();
        else if (Enemy.CurrentHP <= 0) yield return Win();
    }

    yield return new WaitForSeconds(0.3f);
    
    // 🟢 PENTING: Panggil session.EndBattle SEBELUM scenes.EndBattle
    if (session != null)
    {
        session.EndBattle(result);
    }
    
    if (scenes != null)
    {
        scenes.EndBattle(result);
    }
    }

    private IEnumerator Turn(BattleAction action, MonsterInstanceI user, MonsterInstanceI target)
    {
        if (user.CurrentHP <= 0 || target.CurrentHP <= 0 || battleEnded) yield break;
        yield return action.Execute(this, user, target);
    }

    private BattleAction PickEnemyAction()
    {
        return UnityEngine.Random.value < 0.35f ? (BattleAction)new SkillAction() : new AttackAction();
    }

    public IEnumerator Say(string text, float hold = 0.8f)
    {
        MessageShown?.Invoke(text);
        float wait = hold + text.Length * secondsPerChar;
        for (float t = 0f; t < wait; t += Time.deltaTime)
        {
            if (battleEnded) yield break;
            if (t > text.Length * secondsPerChar && MenuInput.Confirm) break;
            yield return null;
        }
    }

    public void RaiseAttacked(MonsterInstanceI m) => Attacked?.Invoke(m);
    public void RaiseHit(MonsterInstanceI target, int damage, bool critical, Element? element) => Hit?.Invoke(target, damage, critical, element);
    public void RaiseMissed() => Missed?.Invoke();
    
    public void SetFled() => HasFled = true;

    public float DamageScale(MonsterInstanceI user) => user == Enemy && !IsBoss ? 0.8f : 1f;

    private IEnumerator Win()
    {
        result = BattleResultI.Win;
        Fainted?.Invoke(Enemy);
        yield return new WaitForSeconds(0.6f);

        yield return Say(IsBoss ? $"{Enemy.Data.DisplayName} kalah!" : $"{Enemy.Data.DisplayName} liar kalah!", 0.8f);

        int exp = (10 + Enemy.Level * 8) * (IsBoss ? 2 : 1);
        yield return Say($"{Player.Data.DisplayName} mendapat {exp} EXP.", 0.9f);

        foreach (GrowthEvent growth in Player.GainExp(exp))
        {
            if (growth.Type == GrowthType.LevelUp)
            {
                LevelUp?.Invoke(growth.From);
                yield return Say(growth.Message, 0.9f);
                continue;
            }
            yield return Say($"Apa? {growth.From.DisplayName} mulai bersinar...", 0.9f);
            Evolving?.Invoke(growth.From, growth.To);
            yield return new WaitForSeconds(2.4f);
            Evolved?.Invoke(Player);
            yield return Say(growth.Message, 1.2f);
        }

        if (!IsBoss)
        {
            int healed = Player.Heal(Mathf.RoundToInt(Player.MaxHP * 0.25f));
            if (healed > 0) yield return Say($"{Player.Data.DisplayName} beristirahat sejenak dan pulih {healed} HP.", 0.8f);
        }
    }

    private IEnumerator Lose()
    {
       result = BattleResultI.Lose;
        Fainted?.Invoke(Player);
        yield return new WaitForSeconds(0.6f);

        yield return Say($"{Player.Data.DisplayName} pingsan... Kamu berlari kembali ke desa.", 1.0f);

        // 🟢 HP Partner dipulihkan tepat ke 50%
        int targetHP = Mathf.RoundToInt(Player.MaxHP * 0.5f);
        int diff = targetHP - Player.CurrentHP;
        
        if (diff > 0)
        {
            Player.Heal(diff);
        }

        Debug.Log($"[BattleSystem] Player Lose. Partner HP set to {Player.CurrentHP}/{Player.MaxHP}");

        // 🟢 Teleport ke SaveStone & reset monster
        if (scenes != null) 
        {
            scenes.TeleportToSaveStone();
        }
    }
}