using System.Collections.Generic;
using UnityEngine;

public class BattlePresenter
{
    private BattleSystem battle;
    private BattleView view;
    private int lastPick = 0;

    public BattlePresenter(BattleSystem battleSystem, BattleView battleView)
    {
        this.battle = battleSystem;
        this.view = battleView;

        // Hubungkan event - SESUAI MODUL P5 SLIDE 33
        battle.Started += OnStarted;
        battle.MessageShown += view.ShowMessage;
        battle.ChoiceRequested += OnChoiceRequested;
        battle.Attacked += m => PanelOf(m).PlayLunge(m == battle.Player);
        battle.Hit += OnHit;
        battle.Missed += () => view.ShowMessage("Serangan meleset!");
        battle.Fainted += m => PanelOf(m).PlayFaint();
        battle.LevelUp += shown => view.PlayerPanel.Show(battle.Player, false, shown);
        battle.Evolving += (from, to) =>
        {
            view.PlayerPanel.PlayEvolve(from, to, 2.4f);
            view.PlayEvolveRings(2.4f);
        };
        battle.Evolved += m =>
        {
            view.PlayFlash();
            view.PlayerPanel.Show(m, false);
        };

        view.ActionClicked += OnActionClicked;
    }

    private void OnStarted()
    {
        view.PlayerPanel.Show(battle.Player, false);
        view.EnemyPanel.Show(battle.Enemy, true);
        view.PlayIntro();
    }

    private void OnChoiceRequested()
    {
        int potionCount = battle.Party.Potions;
        var labels = new List<string> { "Serang", "Jurus", $"Ramuan ({potionCount})", "Kabur" };
        var usable = new List<bool> { true, true, potionCount > 0, !battle.IsBoss };
        string hint = $"Jurus elemen: {battle.Player.Data.SkillName}";
        view.ShowMenu(labels, usable, $"Apa yang akan {battle.Player.Data.DisplayName} lakukan?", hint);
    }

    public void OnActionClicked(int index)
    {
        lastPick = index;
        switch (index)
        {
            case 0: battle.ChosenAction = new AttackAction(); break;
            case 1: battle.ChosenAction = new SkillAction(); break;
            case 2: battle.ChosenAction = new PotionAction(); break;
            case 3: battle.ChosenAction = new RunAction(); break;
        }
    }

    private void OnHit(MonsterInstanceI target, int damage, bool critical, Element? element)
    {
        PanelOf(target).PlayHurt();
        string msg = critical ? "Serangan kritis!" : "Serangan mengenai!";
        view.ShowMessage(msg);
    }

    private MonsterPanel PanelOf(MonsterInstanceI m)
    {
        return m == battle.Player ? view.PlayerPanel : view.EnemyPanel;
    }
}