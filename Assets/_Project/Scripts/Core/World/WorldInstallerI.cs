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
            // Set player transform ke party untuk teleport
            if (s.Party != null)
            {
                s.Party.SetPlayerTransform(player.transform);
            }
        }
        else Debug.LogError("[WorldInstaller] player NULL di Inspector!");

        if (interactor != null) interactor.Construct(s.Input);
        else Debug.LogError("[WorldInstaller] interactor NULL di Inspector!");

        if (follower != null) follower.Construct(s.Party, player);

        // 2. Handle Save Load atau Intro Pembuka (Sesuai Modul P6 Slide 22)
        if (s.Save != null && s.Save.PendingLoad != null)
        {
            // Jika ada data load, teleport player ke posisi tersimpan
            if (player != null)
            {
                player.transform.position = s.Save.PendingLoad.playerPosition;
            }
            s.Save.PendingLoad = null; // Clear pending load agar tidak loop

            if (s.Toast != null) s.Toast.Show("Perjalanan dilanjutkan");
        }
        else if (s.Flags != null && !s.Flags.Has(StoryFlags.IntroSeen))
        {
            // Jalankan narasi pembuka jika ini permainan baru
            if (s.Scenes != null) s.Scenes.HoldFade = true; // Tahan fade-in
            StartCoroutine(RunIntro(s));
        }

        // 3. NPC Construct (dengan fallback aman untuk Dialogue)
        DialogueRunner dialogue = s.Dialogue;
        if (dialogue == null)
        {
            Debug.LogWarning("[WorldInstaller] s.Dialogue NULL! Mencari via FindAnyObjectByType...");
            dialogue = FindAnyObjectByType<DialogueRunner>();
            if (dialogue != null) s.Dialogue = dialogue;
        }

        if (starterGiver != null) starterGiver.Construct(dialogue, s.Flags, s.Party);
        if (guardNpc != null) guardNpc.Construct(dialogue, s.Flags, s.Party);
        if (merchantNpc != null) merchantNpc.Construct(dialogue, s.Flags, s.Party);

        foreach (NpcI npc in otherNpcs)
        {
            if (npc != null) npc.Construct(dialogue, s.Flags, s.Party);
        }

        // 4. Monster Liar
        WildMonsterI[] monsters = FindObjectsByType<WildMonsterI>();
        Debug.Log($"[WorldInstaller] Menemukan {monsters.Length} monster liar");

        foreach (WildMonsterI wild in monsters)
        {
            // 🟢 PERBAIKAN: Tambahkan parameter ke-4 (s.Input)
            wild.Construct(s.Battle, s.Scenes, player.transform, s.Input);
        }
    }

    // Narasi pembuka sesuai modul P6 (slide 22-23)
    private IEnumerator RunIntro(GameServicesI s)
    {
        // Narasi pembuka - kotak biru tanpa nama pembicara
        yield return s.Dialogue.Narrate("Di ujung Pulau Batam ada desa kecil yang damai...");
        yield return s.Dialogue.Narrate("Namun, hutan di utara dipenuhi monster liar yang berbahaya.");

        yield return null;

        // Tunggu sampai seluruh dialog intro selesai
        while (s.Dialogue.IsRunning) yield return null;

        // Tandai intro sudah dilihat agar tidak muncul lagi
        s.Flags.Set(StoryFlags.IntroSeen);

        // Fade-in setelah intro selesai
        if (s.Scenes != null) yield return s.Scenes.ReleaseFade();

        if (s.Toast != null)
        {
            s.Toast.Show("Temui Tetua Rahmat (kanan jalan desa)");
        }
    }
}