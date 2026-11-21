using UnityEngine;

public class BattleInstaller : SceneInstaller
{
    [SerializeField] private BattleView view;
    [SerializeField] private SpriteRenderer forestBackground;
    [SerializeField] private SpriteRenderer caveBackground;
    [SerializeField] private BattleFeel battleFeel;

    public override void Install(GameServices s)
    {
        if (forestBackground != null) forestBackground.enabled = !s.Battle.IsBoss;
        if (caveBackground != null) caveBackground.enabled = s.Battle.IsBoss;

        var battle = new BattleSystem();
        battle.Construct(s.Party, s.Battle, s.Scenes);
        new BattlePresenter(battle, view, s.Audio); 
        battle.Begin();

        if (battleFeel != null)
        battleFeel.Construct(battle);

    }
}