using UnityEngine;

public class GameBootstrapI : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string firstScene = "Scn_WorldI";
    
    [Header("Data")]
    [SerializeField] private MonsterDatabaseI monsterDatabase;
    
    [Header("UI")]
    [SerializeField] private ScreenFader fader;
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private ToastView toastView;
    
    private GameServicesI services;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Debug.Log("🚀 1. GameBootstrapI AWAKE BERJALAN!");

        // 🟢 WAJIB: Buat DialogueRunner & ToastView juga DontDestroyOnLoad
        if (dialogueRunner != null)
        {
            DontDestroyOnLoad(dialogueRunner.gameObject);
            Debug.Log($"[GameBootstrap] DialogueRunner DontDestroyOnLoad: {dialogueRunner.gameObject.name}");
        }
        else
        {
            Debug.LogError("[GameBootstrap] dialogueRunner NULL di Inspector!");
        }

        if (toastView != null)
        {
            DontDestroyOnLoad(toastView.gameObject);
            Debug.Log($"[GameBootstrap] ToastView DontDestroyOnLoad: {toastView.gameObject.name}");
        }

        var gate = new InputGate(new InputSystemMoveInputI(), new InputSystemActionInputI());
        
        services = new GameServicesI
        {
            Input = gate,
            Database = monsterDatabase,
            Party = new PlayerPartyI(),
            Battle = new BattleSessionI(),
            Dialogue = dialogueRunner,
            Flags = new StoryFlags(),
            Toast = toastView
        };
        
        services.Scenes = gameObject.AddComponent<SceneFlowI>();
        services.Scenes.Construct(this, fader, services);
        
        if (dialogueRunner != null)
        {
            dialogueRunner.SetGate(gate);
        }
    }

    void Start()
    {
        Debug.Log("🚀 2. GameBootstrapI START BERJALAN!");
        services.Scenes.GoTo(firstScene);
    }
    
    public GameServicesI Services => services;
}