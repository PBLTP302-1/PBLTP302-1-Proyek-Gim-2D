using UnityEngine;

public class GameBootstrapI : MonoBehaviour
{
    public static GameServicesI CurrentServices { get; private set; }

    [Header("Konfigurasi Awal")]
    [SerializeField] private string firstScene = "Scn_Title";
    [SerializeField] private MonsterDatabaseI monsterDatabase;
    [SerializeField] private ScreenFader fader;

    [Header("UI References")]
    [SerializeField] private ToastView toastView;
    [SerializeField] private DialogueRunner dialogueRunner;

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    private GameServicesI services;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Debug.Log("1. GameBootstrapI AWAKE BERJALAN!");

        var gate = new InputGate(
            new InputSystemMoveInputI(), 
            new InputSystemActionInputI()
        );

        if (toastView == null)
        {
            toastView = FindAnyObjectByType<ToastView>();
        }

        if (dialogueRunner == null)
        {
            dialogueRunner = FindAnyObjectByType<DialogueRunner>();
        }

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
            Audio = new AudioService(), // 🟢 Inisialisasi AudioService
            Scenes = null
        };

        CurrentServices = services;

        services.Scenes = FindAnyObjectByType<SceneFlowI>();

        if (services.Scenes == null)
        {
            Debug.LogWarning("[GameBootstrapI] ⚠️ SceneFlowI tidak ditemukan! Membuat GameObject otomatis...");
            
            GameObject sceneFlowObj = new GameObject("_SceneFlow");
            sceneFlowObj.transform.SetParent(transform);
            services.Scenes = sceneFlowObj.AddComponent<SceneFlowI>();
            
            Debug.Log("[GameBootstrapI] ✅ SceneFlowI dibuat otomatis");
        }
        else
        {
            Debug.Log("[GameBootstrapI] ✅ SceneFlowI ditemukan di scene");
        }

        services.Scenes.Construct(this, fader, services);
        Debug.Log("[GameBootstrapI] ✅ SceneFlowI berhasil di-construct");

        // 🟢 Inisialisasi AudioService
        if (services.Audio != null)
        {
            services.Audio.Init(musicSource, sfxSource);
            Debug.Log("[GameBootstrapI] ✅ AudioService berhasil di-inisialisasi");
        }
        else
        {
            Debug.LogError("[GameBootstrapI] ❌ AudioService NULL!");
        }

        if (services.Dialogue != null)
        {
            services.Dialogue.SetGate(services.Input);
        }
        else
        {
            Debug.LogError("[GameBootstrapI] ❌ DialogueRunner NULL!");
        }

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
        Debug.Log("2. GameBootstrapI START BERJALAN!");

        if (services.Scenes == null)
        {
            Debug.LogError("[GameBootstrapI] ❌ Gagal memuat scene karena SceneFlowI NULL.");
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

    void Update()
    {
        var keyboard = UnityEngine.InputSystem.Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.mKey.wasPressedThisFrame)
        {
            if (services.Audio != null)
            {
                bool isMuted = services.Audio.ToggleMute();
                Debug.Log($"[GameBootstrapI] Mute: {isMuted}");
                
                if (services.Toast != null)
                {
                    services.Toast.Show(isMuted ? "🔇 Suara dimatikan" : "🔊 Suara diaktifkan");
                }
            }
        }
    }

    public GameServicesI GetServices() => services;
}