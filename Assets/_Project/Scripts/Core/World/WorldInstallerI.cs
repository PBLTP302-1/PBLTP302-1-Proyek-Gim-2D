using System.Collections;
using UnityEngine;

public class WorldInstallerI : SceneInstaller
{
    [Header("Player & Partner")]
    [SerializeField] private PlayerMovementI player;
    [SerializeField] private PlayerInteractorI interactor;
    [SerializeField] private PartnerFollower follower;

    [Header("NPC P6")]
    [SerializeField] private StarterGiver starterGiver;
    [SerializeField] private GuardNpc guardNpc;
    [SerializeField] private MerchantNpc merchantNpc;
    [SerializeField] private NpcI[] otherNpcs;

    [Header("World Objects")]
    [SerializeField] private SaveStone saveStone;

    public override void Install(GameServicesI s)
    {
        Debug.Log("[WorldInstaller] Install() dipanggil!");

        // 1. Player & Interactor
        if (player != null)
        {
            player.Construct(s.Input);
            if (s.Party != null)
            {
                s.Party.SetPlayerTransform(player.transform);
            }
        }
        else Debug.LogError("[WorldInstaller] player NULL di Inspector!");

        if (interactor != null) interactor.Construct(s.Input);
        else Debug.LogError("[WorldInstaller] interactor NULL di Inspector!");

        if (follower != null) follower.Construct(s.Party, player);

        // 2. Handle Save Load atau Intro Pembuka
        if (s.Save != null && s.Save.PendingLoad != null)
        {
            Debug.Log("[WorldInstaller] Memuat data dari Save File...");
            
            SaveMapper.Apply(s.Save.PendingLoad, s);
            
            if (player != null)
            {
                player.transform.position = s.Save.PendingLoad.playerPosition;
            }
            
            s.Save.PendingLoad = null;

            if (s.Toast != null) s.Toast.Show("Perjalanan dilanjutkan");
        }
        else if (s.Flags != null && !s.Flags.Has(StoryFlags.IntroSeen))
        {
            if (s.Scenes != null) s.Scenes.HoldFade = true;
            StartCoroutine(RunIntro(s));
        }

        // 3. NPC Construct
        DialogueRunner dialogue = s.Dialogue;
        if (dialogue == null)
        {
            Debug.LogWarning("[WorldInstaller] s.Dialogue NULL! Mencari via FindAnyObjectByType...");
            dialogue = FindAnyObjectByType<DialogueRunner>();
            if (dialogue != null) s.Dialogue = dialogue;
        }

        if (starterGiver != null) starterGiver.Construct(dialogue, s.Flags, s.Party, s.Toast);
        if (guardNpc != null) guardNpc.Construct(dialogue, s.Flags, s.Party, s.Toast);
        if (merchantNpc != null) merchantNpc.Construct(dialogue, s.Flags, s.Party, s.Toast);

        foreach (NpcI npc in otherNpcs)
        {
            if (npc != null) npc.Construct(dialogue, s.Flags, s.Party, s.Toast);
        }

        // 4. SaveStone Construct
        if (saveStone != null)
        {
            saveStone.Construct(s, player);
            Debug.Log("[WorldInstaller] SaveStone berhasil di-construct.");
        }
        else
        {
            Debug.LogWarning("[WorldInstaller] SaveStone NULL di Inspector!");
        }

        // 🟢 5. Monster Liar - DENGAN NULL-CHECK LENGKAP
        Debug.Log($"[WorldInstaller] s.Scenes = {s.Scenes != null}");
        Debug.Log($"[WorldInstaller] s.Battle = {s.Battle != null}");
        
        WildMonsterI[] monsters = FindObjectsByType<WildMonsterI>();
        Debug.Log($"[WorldInstaller] Menemukan {monsters.Length} monster liar");

        foreach (WildMonsterI wild in monsters)
        {
            if (s.Scenes == null)
            {
                Debug.LogError($"[WorldInstaller] s.Scenes NULL! Monster {wild.gameObject.name} tidak bisa di-construct.");
                continue;
            }

            if (s.Battle == null)
            {
                Debug.LogError($"[WorldInstaller] s.Battle NULL! Monster {wild.gameObject.name} tidak bisa di-construct.");
                continue;
            }

            wild.Construct(s.Battle, s.Scenes, player.transform, s.Input);
            Debug.Log($"[WorldInstaller] Monster {wild.gameObject.name} berhasil di-construct");
        }
    }

    private IEnumerator RunIntro(GameServicesI s)
    {
        Debug.Log("[WorldInstaller] Memulai narasi pembuka...");

        s.Dialogue.Run(IntroLines());
        yield return null;

        while (s.Dialogue.IsRunning)
        {
            yield return null;
        }

        Debug.Log("[WorldInstaller] Narasi pembuka selesai.");

        s.Flags.Set(StoryFlags.IntroSeen);

        if (s.Scenes != null)
        {
            yield return s.Scenes.ReleaseFade();
        }

        if (s.Toast != null)
        {
            Debug.Log("[WorldInstaller] Menampilkan toast: Temui Tetua Rahmat");
            s.Toast.Show("Temui Tetua Rahmat (kanan jalan desa)");
        }
        else
        {
            Debug.LogError("[WorldInstaller] ToastView NULL!");
        }
    }

    private IEnumerator IntroLines()
    {
        DialogueRunner dialogue = FindAnyObjectByType<DialogueRunner>();
        
        yield return dialogue.Narrate("Di ujung Pulau Batam ada desa kecil yang damai...");
        yield return dialogue.Narrate("Namun, hutan di utara dipenuhi monster liar yang berbahaya.");
    }
}