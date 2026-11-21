using System.Collections;
using UnityEngine;

public class WorldInstaller : SceneInstaller
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private PlayerInteractor interactor;
    [SerializeField] private PartnerFollower follower;
    [SerializeField] private MonsterData debugStarter;
    [SerializeField] private PartnerHud partnerHud;
    [SerializeField] private Npc[] npcs;
    [SerializeField] private FlagToggle[] flagToggles;
    [SerializeField] private Vector2 respawnPoint = new Vector2(21.5f, 6f);
    private GameServices services;
    public override void Install(GameServices s)
    {
        services = s;

        player.Construct(s.Input);
        interactor.Construct(s.Input);
        follower.Construct(s.Party, player);

        foreach (var wild in FindInScene<WildMonster>())
            wild.Construct(s.Battle, s.Scenes, s.Party, player.transform);

        foreach (var npc in npcs)
        {
            npc.Construct(s.Dialogue, s.Flags, s.Toast); 
            if (npc is StarterGiver sg) sg.SetParty(s.Party);
            if (npc is MerchantNpc mn) mn.SetParty(s.Party);
        }

                foreach (var saveStone in FindInScene<SaveStone>())
        {
            saveStone.Construct(s);
        }

        foreach (var ft in flagToggles)
            ft.Construct(s.Flags);
        
        s.Battle.Ended += OnBattleEnded;

        if (partnerHud != null) partnerHud.Construct(s.Party);

        if (s.Save.PendingLoad != null)
        {
            player.Teleport(s.Save.PendingLoad.playerPosition, Vector2.down);
            s.Save.PendingLoad = null;
            if (s.Toast != null) s.Toast.Show("Perjalanan dilanjutkan");
        }
        else if (!s.Flags.Has(StoryFlags.IntroSeen))
        {
            s.Scenes.HoldFade = true;
            StartCoroutine(Intro(s));
        }

        if (s.Party.Partner == null)
        {
            if (debugStarter != null)
            {
                s.Party.SetPartner(new MonsterInstance(debugStarter, 3));
                Debug.Log($"[WorldInstaller] ✅ Debug starter: {debugStarter.DisplayName} Lv3");
            }
            else if (s.Flags.Has(StoryFlags.HasStarter))
            {
                Debug.LogError("[WorldInstaller] ❌ Flag has_starter aktif tapi partner null! Cek SaveMapper.");
            }
            else
            {
                Debug.LogWarning("[WorldInstaller] ⚠️ Tidak ada partner. Pemain HARUS bicara dengan Tetua untuk dapat partner.");
            }
        }
    }

    void OnDestroy()
    {
        if (services != null && services.Battle != null)
        {
            services.Battle.Ended -= OnBattleEnded;
        }
    }


    private void OnBattleEnded(BattleResult result, bool wasBoss)
    {
        if (result == BattleResult.Lose)
        {
            player.Teleport(respawnPoint, Vector2.up);
            if (services.Toast != null) services.Toast.Show("Kalah... kembali ke desa.");
        }
        else if (result == BattleResult.Win && wasBoss)
        {
            StartCoroutine(Ending());
        }
    }


    private IEnumerator Intro(GameServices s)
    {
        if (s.Dialogue == null)
        {
            Debug.LogError("[WorldInstaller] DialogueRunner tidak ada! Skip intro.");
            s.Flags.Set(StoryFlags.IntroSeen);
            yield return s.Scenes.ReleaseFade();
            yield break;
        }

        s.Dialogue.Run(IntroLines(s));
        yield return null;

        while (s.Dialogue.IsRunning)
        {
            yield return null;
        }

        s.Flags.Set(StoryFlags.IntroSeen);
        yield return s.Scenes.ReleaseFade();

        if (s.Toast != null)
        {
            s.Toast.Show("Temui Tetua Rahmat (kanan jalan desa)");
        }
    }

    private IEnumerator IntroLines(GameServices s)
    {
        yield return s.Dialogue.Narrate("Di ujung Pulau Batam ada desa kecil yang dikelilingi hutan lebat.");
        yield return s.Dialogue.Narrate("Konon, monster liar berkeliaran di sana. Hanya yang punya partner yang boleh masuk.");
    }

    private IEnumerator Ending()
    {
        while (services.Scenes.IsBusy) yield return null;
        services.Dialogue.Run(EndingLines());
        yield return null;
        while (services.Dialogue.IsRunning) yield return null;
        
        services.Scenes.GoTo(SceneFlow.EndScene);
    }

    private IEnumerator EndingLines()
    {
        yield return services.Dialogue.Narrate("Raijugar menunduk dan membiarkanmu lewat...");
        yield return services.Dialogue.Narrate("Cahaya kristal menerangi jalan keluar gua.");
        yield return services.Dialogue.Narrate("Petualanganmu di Pulau Monster telah mencapai puncak.");
    }

}
