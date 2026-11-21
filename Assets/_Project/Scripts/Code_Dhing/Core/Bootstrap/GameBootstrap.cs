using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string firstScene = "Scn_Title";
    [SerializeField] private ScreenFader fader;

    [Header("Database")]
    [SerializeField] private MonsterDatabase monsterDatabase;

    [Header("UI")]
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private ToastView toastView;
    [SerializeField] private PauseMenu pauseMenu; 


    private GameServices services;
    private float lastTime;


    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // === Input ===
        var gate = new InputGate(
            new InputSystemMoveInput(),
            new InputSystemActionInput()
        );

        // === Core Services ===
        var flags = new StoryFlags();
        var save = new SaveService();

        services = new GameServices
        {
            Input = gate,
            Database = monsterDatabase,
            Party = new PlayerParty(),
            Battle = new BattleSession(),
            Flags = flags,
            Save = save,
        };

        services.Scenes = new SceneFlow(this, fader, services);

        // === Dialogue ===
        if (dialogueRunner != null)
        {
            dialogueRunner.Construct(gate);
            services.Dialogue = dialogueRunner;
        }

        // === Toast ===
        if (toastView != null)
        {
            toastView.gameObject.SetActive(false);
            services.Toast = toastView;
        }

        if (pauseMenu != null)
        {
            pauseMenu.Construct(services);
        }

        // === Audio (cari di scene Bootstrap) ===
        var audioService = FindAnyObjectByType<AudioService>();
        if (audioService != null)
        {
            services.Audio = audioService;
        }
        else
        {
            Debug.LogWarning("[GameBootstrap] AudioService tidak ditemukan di scene!");
        }
        
        lastTime = Time.unscaledTime;

    }

    void Start() => services.Scenes.GoTo(firstScene);

    void Update()
    {
        if (services.Scenes.CurrentScene == SceneFlow.WorldScene || services.Scenes.InBattle)
        {
            float delta = Time.unscaledTime - lastTime;
            services.Party.AddPlaytime(delta);
        }
        lastTime = Time.unscaledTime;

        if (MenuInput.Mute && services.Audio != null)
        {
            bool muted = services.Audio.ToggleMute();
            if (services.Toast != null)
                services.Toast.Show(muted ? "Suara dimatikan" : "Suara dinyalakan");
        }
    }
}