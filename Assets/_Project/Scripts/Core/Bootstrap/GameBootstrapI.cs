using UnityEngine;

public class GameBootstrapI : MonoBehaviour
{
    // 🟢 STATIC REFERENCE untuk TitleInstaller
    public static GameServicesI CurrentServices { get; private set; }

    [Header("Konfigurasi Awal")]
    [SerializeField] private string firstScene = "Scn_Title"; 
    [SerializeField] private MonsterDatabaseI monsterDatabase;
    [SerializeField] private ScreenFader fader;

    [Header("UI References")]
    [SerializeField] private ToastView toastView;
    [SerializeField] private DialogueRunner dialogueRunner;

    private GameServicesI services;

    void Awake()
    {
        // 🟢 PENTING: Agar DialogueRunner & ToastView tidak hancur saat ganti scene
        DontDestroyOnLoad(gameObject);

        // 1. Buat Input Gate
        var gate = new InputGate(
            new InputSystemMoveInputI(), 
            new InputSystemActionInputI()
        );

        // 2. Cari ToastView
        if (toastView == null)
        {
            toastView = FindAnyObjectByType<ToastView>();
        }

        // 3. Cari DialogueRunner
        if (dialogueRunner == null)
        {
            dialogueRunner = FindAnyObjectByType<DialogueRunner>();
        }

        // 4. Inisialisasi GameServicesI
        services = new GameServicesI
        {
            Input = gate,
            Database = monsterDatabase,
            Party = new PlayerPartyI(),
            Battle = new BattleSessionI(),
            Flags = new StoryFlags(),
            Toast = toastView,
            Dialogue = dialogueRunner,
            Save = new SaveService(),
            Audio = null
        };
        
        // 🟢 Set static reference
        CurrentServices = services;

        // 5. Cari SceneFlowI dari scene (CARA AMAN untuk MonoBehaviour)
        services.Scenes = FindObjectOfType<SceneFlowI>();
        if (services.Scenes != null)
        {
            services.Scenes.Construct(this, fader, services);
            Debug.Log("[GameBootstrapI] ✅ SceneFlowI ditemukan & berhasil di-construct.");
        }
        else
        {
            Debug.LogError("[GameBootstrapI] ❌ SceneFlowI TIDAK DITEMUKAN!");
            Debug.LogError("[GameBootstrapI] Pastikan ada GameObject bernama '_SceneFlow' dengan komponen SceneFlowI di scene Scn_Bootstrap.");
        }

        // 6. Hubungkan DialogueRunner dengan InputGate
        if (services.Dialogue != null)
        {
            services.Dialogue.SetGate(services.Input);
        }
        else
        {
            Debug.LogError("[GameBootstrapI] ❌ DialogueRunner NULL!");
        }

        // 7. Verifikasi
        if (services.Save == null)
        {
            Debug.LogError("[GameBootstrapI] ❌ SaveService NULL!");
        }
        else
        {
            Debug.Log("[GameBootstrapI] ✅ SaveService berhasil di-inisialisasi");
        }

        if (services.Toast == null)
        {
            Debug.LogError("[GameBootstrapI] ❌ ToastView NULL!");
        }
    }

    void Start()
    {
        // 🟢 SAFETY CHECK: Pastikan Scenes tidak null sebelum dipanggil
        if (services.Scenes == null)
        {
            Debug.LogError("[GameBootstrapI] ❌ Gagal memuat scene karena SceneFlowI NULL. Game berhenti.");
            return;
        }

        if (!string.IsNullOrEmpty(firstScene))
        {
            Debug.Log($"[GameBootstrapI] Memulai game, memuat scene: {firstScene}");
            services.Scenes.GoTo(firstScene);
        }
        else
        {
            Debug.LogError("[GameBootstrapI] ❌ firstScene kosong!");
        }
    }

    public GameServicesI GetServices() => services;
}