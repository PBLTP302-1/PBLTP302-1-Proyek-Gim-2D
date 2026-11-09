public class GameServicesI
{
    public InputGate Input { get; set; }
    public MonsterDatabaseI Database { get; set; }
    public PlayerPartyI Party { get; set; }
    public BattleSessionI Battle { get; set; }
    public StoryFlags Flags { get; set; }
    public ToastView Toast { get; set; }
    public DialogueRunner Dialogue { get; set; }
    public SaveService Save { get; set; }
    public AudioService Audio { get; set; } // 🟢 TAMBAHKAN INI
    public SceneFlowI Scenes { get; set; }
}