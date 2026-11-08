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

    if (dialogueRunner != null)
    {
        DontDestroyOnLoad(dialogueRunner.gameObject);
    }

    if (toastView != null)
    {
        DontDestroyOnLoad(toastView.gameObject);
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
        Toast = toastView,
        Save = new SaveService(),      // 🟢 TAMBAHKAN
        Audio = new AudioService()     // 🟢 TAMBAHKAN
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