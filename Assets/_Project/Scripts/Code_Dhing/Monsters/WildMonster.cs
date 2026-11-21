using System.Collections;
using UnityEngine;

public class WildMonster : MonoBehaviour
{
    [SerializeField] private MonsterData species;
    [SerializeField] private Vector2Int levelRange = new Vector2Int(2, 4);
    [SerializeField] private bool isBoss;
    [SerializeField] private bool stationary;
    [SerializeField] private float wanderSpeed = 1.125f;
    [SerializeField] private float chaseSpeed = 2.5f;
    [SerializeField] private float sightRadius = 3.25f;
    [SerializeField] private float giveUpRadius = 6.25f;
    [SerializeField] private float homeRadius = 3f;
    [SerializeField] private float respawnSeconds = 20f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer body;
    [SerializeField] private Collider2D hitbox;
    [SerializeField] private GameObject alert;

    private DialogueRunner dialogue;
    private BattleSession session;
    private SceneFlow scenes;
    private PlayerParty party;
    private StateMachine fsm;
    private Vector2 desiredVelocity;
    private bool inBattle;
    private bool hidden;
    private float cooldown;

    public Transform Target { get; private set; }
    public Vector2 Home { get; private set; }
    public float WanderSpeed => wanderSpeed;
    public float ChaseSpeed => chaseSpeed;
    public float HomeRadius => homeRadius;
    public bool IsBoss => isBoss;
    public WanderState Wander { get; private set; }
    public ChaseState Chase { get; private set; }
    public ReturnState Return { get; private set; }

    public void Construct(BattleSession battleSession, SceneFlow sceneFlow, PlayerParty playerParty, Transform player)
    {
        session = battleSession;
        scenes = sceneFlow;
        party = playerParty;
        Target = player;
        dialogue = FindAnyObjectByType<DialogueRunner>();
    }

    void Awake()
    {
        Home = transform.position;
        fsm = new StateMachine();
        Wander = new WanderState(this);
        Chase = new ChaseState(this);
        Return = new ReturnState(this);
    }

    void Start() => fsm.ChangeTo(Wander);

    void Update()
    {
        if (cooldown > 0f) cooldown -= Time.deltaTime;

        // ✅ Monster hidden (sedang respawn) atau sedang battle → diam
        if (hidden || body == null || !body.enabled || stationary || inBattle || Target == null)
        {
            desiredVelocity = Vector2.zero;
            return;
        }

        fsm.Tick(Time.deltaTime);

        if (alert != null) alert.SetActive(fsm.Current == Chase && !inBattle);
        if (desiredVelocity.x != 0f) body.flipX = desiredVelocity.x < 0f;
    }

    void FixedUpdate() => rb.linearVelocity = desiredVelocity;

    public void Move(Vector2 velocity) => desiredVelocity = velocity;
    public void ChangeState(IState next) => fsm.ChangeTo(next);
    public float DistanceToTarget => Vector2.Distance(transform.position, Target.position);

    private bool Calm => cooldown > 0f || session.IsSafe;
    public bool CanSeeTarget => party != null && party.Partner != null && !Calm && DistanceToTarget < sightRadius;
    public bool LostTarget => Calm || DistanceToTarget > giveUpRadius;

    void OnCollisionEnter2D(Collision2D collision) => TryEncounter(collision);
    void OnCollisionStay2D(Collision2D collision) => TryEncounter(collision);

    private void TryEncounter(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) StartEncounter();
    }

    private void StartEncounter()
    {
        if (hidden || inBattle || Calm || scenes.IsBusy || party == null || party.Partner == null) return;
        inBattle = true;
        int level = Random.Range(levelRange.x, levelRange.y + 1);
        session.Begin(new MonsterInstance(species, level), isBoss, OnBattleFinished);

        if (isBoss && dialogue != null)
        {
            dialogue.Run(Roar());
        }
        else
        {
            scenes.StartBattle();
        }
    }

    private IEnumerator Roar()
    {
        yield return dialogue.Say("Raijugar", "GRRRAAAH!! Raijugar menghalangi jalan, percikan listrik memenuhi gua!");
        scenes.StartBattle();
    }

    private void OnBattleFinished(BattleResult result)
    {
        inBattle = false;
        if (result == BattleResult.Win)
        {
            if (isBoss) { gameObject.SetActive(false); return; }

            hidden = true;
            SetVisible(false);
            CoroutineRunner.Instance.StartCoroutine(RespawnRoutine());
        }
        else
        {
            cooldown = 2.5f;
            fsm.ChangeTo(Return);
        }
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnSeconds);
        RespawnNow();
    }

    private void RespawnNow()
    {
        transform.position = Home;
        fsm.ChangeTo(Wander);
        hidden = false;
        cooldown = 1.0f;
        SetVisible(true);
    }

    private void SetVisible(bool visible)
    {
        if (body != null) body.enabled = visible;
        if (hitbox != null) hitbox.enabled = visible;
        desiredVelocity = Vector2.zero;
        if (alert != null) alert.SetActive(false);
    }
}