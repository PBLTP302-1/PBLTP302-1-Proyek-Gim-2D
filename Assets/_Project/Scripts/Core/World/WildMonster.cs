using UnityEngine;

public class WildMonster : MonoBehaviour
{
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private int level = 3;
    [SerializeField] private float sightRadius = 3.25f;
    [SerializeField] private float giveUpRadius = 6.25f;
    [SerializeField] private GameObject alertSign; // Sprite tanda seru

    private StateMachine fsm;
    private Transform playerTransform;
    private Vector2 homePosition;
    private MonsterInstance instance;

    void Awake()
    {
        instance = new MonsterInstance(monsterData, level);
        homePosition = transform.position;
        fsm = new StateMachine();
    }

    public void Construct(Transform player)
    {
        playerTransform = player;
        fsm.ChangeTo(new WanderState(this));
    }

    void Update()
    {
        if (playerTransform == null) return;
        
        // Logika transisi state sederhana
        float distToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        float distToHome = Vector2.Distance(transform.position, homePosition);

        if (fsm.Current is WanderState && distToPlayer < sightRadius)
            fsm.ChangeTo(new ChaseState(this));
        else if (fsm.Current is ChaseState && distToPlayer > giveUpRadius)
            fsm.ChangeTo(new ReturnState(this));
        else if (fsm.Current is ReturnState && distToHome < 0.5f)
            fsm.ChangeTo(new WanderState(this));

        fsm.Tick(Time.deltaTime);
    }

    // Helper untuk state
    public void MoveTowards(Vector2 target, float speed, float deltaTime)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * deltaTime);
    }

    public void ShowAlert(bool show) => alertSign.SetActive(show);
    public Vector2 HomePosition => homePosition;
    public Transform PlayerTransform => playerTransform;
}