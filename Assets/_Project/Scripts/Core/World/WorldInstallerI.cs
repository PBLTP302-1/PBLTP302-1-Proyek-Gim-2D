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
            if (player != null)
            {
                player.transform.position = s.Save.PendingLoad.playerPosition;
            }
            s.Save.PendingLoad = null;

            if (s.Toast != null) s.Toast.Show("Perjalanan dilanjutkan");
        }
        else if (s.Flags != null && !s.Flags.Has(StoryFlags.IntroSeen))
        {
            // ⚠️ WAJIB: Tahan fade-in agar layar tetap hitam saat intro
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

        // 4. Monster Liar
        WildMonsterI[] monsters = FindObjectsByType<WildMonsterI>();
        Debug.Log($"[WorldInstaller] Menemukan {monsters.Length} monster liar");

        foreach (WildMonsterI wild in monsters)
        {
            wild.Construct(s.Battle, s.Scenes, player.transform, s.Input);
        }
    }

    // 🟢 METHOD INI YANG HILANG - Narasi pembuka sesuai modul P6
    private IEnumerator RunIntro(GameServicesI s)
    {
        Debug.Log("[WorldInstaller] Memulai narasi pembuka...");

        // Jalankan narasi
        s.Dialogue.Run(IntroLines());

        // ️ WAJIB: Tunggu 1 frame agar Run() sempat dieksekusi
        yield return null;

        // ⚠️ WAJIB: Tunggu sampai seluruh dialog intro selesai
        while (s.Dialogue.IsRunning)
        {
            yield return null;
        }

        Debug.Log("[WorldInstaller] Narasi pembuka selesai.");

        // Tandai intro sudah dilihat agar tidak muncul lagi
        s.Flags.Set(StoryFlags.IntroSeen);

        // ⚠️ WAJIB: Fade-in setelah intro selesai
        if (s.Scenes != null)
        {
            yield return s.Scenes.ReleaseFade();
        }

        // Tampilkan toast petunjuk
        if (s.Toast != null)
        {
            s.Toast.Show("Temui Tetua Rahmat (kanan jalan desa)");
        }
    }

    // 🟢 METHOD INI JUGA HARUS ADA - Urutan narasi pembuka
    private IEnumerator IntroLines()
    {
        DialogueRunner dialogue = FindAnyObjectByType<DialogueRunner>();
        
        yield return dialogue.Narrate("Di ujung Pulau Batam ada desa kecil yang damai...");
        yield return dialogue.Narrate("Namun, hutan di utara dipenuhi monster liar yang berbahaya.");
    }
}