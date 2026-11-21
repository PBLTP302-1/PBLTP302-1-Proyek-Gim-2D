using System.Collections.Generic;
using UnityEngine;

public class BattlePresenter
{
    private BattleSystem battle;
    private BattleView view;
    private AudioService audio;

    public BattlePresenter(BattleSystem battle, BattleView view, AudioService audio)
    {
        this.battle = battle;
        this.view = view;
        this.audio = audio;
        Wire();
    }

    void Wire()
    {
        battle.Started          += OnStarted;
        battle.MessageShown     += ShowMessageSafe;
        battle.ChoiceRequested  += OnChoiceRequested;
        battle.Attacked         += m => PanelOf(m)?.PlayLunge();
        battle.Hit              += OnHit;
        battle.Missed           += OnMissed;
        battle.Fainted          += OnFainted;
        battle.Victory          += OnVictory; 
        battle.LevelUp          += OnLevelUp;
        battle.Evolving         += OnEvolving;
        battle.Evolved          += OnEvolved;
        battle.Healed           += OnHealed;
        view.ActionClicked      += OnActionClicked;
    }

    void ShowMessageSafe(string text) { if (view != null) view.ShowMessage(text); }
    void PlayFlashSafe() { if (view != null) view.PlayFlash(); }

    void OnVictory()
    {
        PlayFlashSafe();
        if (audio != null) audio.PlayVictory();
    }

    void OnStarted()
    {
        if (view == null) return;
        
        if (audio != null)
        {
            audio.PlayEncounter();
            audio.PlayBattleMusic(battle.IsBoss);  
        }

        view.EnemyPanel.Construct(false);
        view.PlayerPanel.Construct(true);
        view.EnemyPanel.Show(battle.Enemy, false);
        view.PlayerPanel.Show(battle.Player, true);
        view.PlayIntro();
        UpdateExpBar();
    }

    void OnMissed()
    {
        ShowMessageSafe("Meleset!");
        if (audio != null) audio.PlayMiss();
    }

    void OnFainted(MonsterInstance m)
    {
        PanelOf(m)?.PlayFaint();
        if (audio != null) audio.PlayFaint();
    }

    void OnLevelUp(MonsterData from)
    {
        if (view != null && view.PlayerPanel != null)
        {
            view.PlayerPanel.Show(battle.Player, true, from);
            UpdateExpBar();
        }
        if (audio != null) audio.PlayLevelUp();
    }

    void OnEvolving(MonsterData from, MonsterData to)
    {
        if (view != null && view.PlayerPanel != null)
        {
            view.PlayerPanel.PlayEvolve(from, to, 2.4f);
            view.PlayEvolveRings(2.4f);
        }
        if (audio != null) audio.PlayEvolve();
    }

    void OnEvolved(MonsterInstance m)
    {
        PlayFlashSafe();
        if (view != null && view.PlayerPanel != null)
            view.PlayerPanel.Show(m);
    }


    void OnHealed(MonsterInstance m)
    {
        if (audio != null) audio.PlayHeal();
    }

    private void UpdateExpBar()
    {
        if (battle.Player != null && battle.Player.ExpToNext > 0 && view != null && view.PlayerPanel != null)
        {
            float ratio = (float)battle.Player.Exp / battle.Player.ExpToNext;
            view.PlayerPanel.PlayExpBar(ratio);
        }
    }

    void OnChoiceRequested()
    {
        if (view == null) return;
        var labels = new List<string> { "Serang", "Jurus", "Ramuan", "Kabur" };
        var usable = new List<bool> { true, true, true, !battle.IsBoss };
        string prompt = $"Apa yang akan {battle.Player.Data.DisplayName} lakukan?";
        string hint = $"Jurus: {battle.Player.Data.SkillName} ({battle.Player.Data.Element})";
        view.ShowMenu(labels, usable, prompt, hint);
    }

    void OnActionClicked(int index)
    {
        if (view != null) view.HideMenu();
        BattleAction action = index switch
        {
            0 => new AttackAction(),
            1 => new SkillAction(),
            2 => new PotionAction(),
            3 => new RunAction(),
            _ => new AttackAction()
        };
        battle.ChooseAction(action);
    }

    void OnHit(MonsterInstance target, int dmg, bool crit, Element? elem)
    {
        PanelOf(target)?.PlayHurt();
        if (audio != null) audio.PlayHit(crit);
    }

    MonsterPanel PanelOf(MonsterInstance m)
    {
        if (view == null) return null;
        return m == battle.Player ? view.PlayerPanel : view.EnemyPanel;
    }
}