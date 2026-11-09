using UnityEngine;

public class WildMonster_Podol : MonoBehaviour
{
    [SerializeField] private MonsterData_Podol monsterData;
    [SerializeField] private int level = 3;
    [SerializeField] private GameObject alert;

    private StateMachine stateMachine;
    private Transform player;
    private Vector2 homePosition;
    private bool inBattle = false;

    void Start()
    {
        homePosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        stateMachine = new StateMachine();
        // Di sini nanti kita atur state Wander, Chase, dan Return
    }

    void Update()
    {
        if (player == null || inBattle) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

       
        if (distanceToPlayer < 3.25f)
        {
            if (alert != null) alert.SetActive(true);
            
        }
        else
        {
            if (alert != null) alert.SetActive(false);
            
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inBattle = true;
            Debug.Log("Menyentuh pemain! Masuk ke Scn_Battle.");
           
        }
    }
}