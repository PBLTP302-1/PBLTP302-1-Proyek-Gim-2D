using UnityEngine;

public class WildMonster_Karin : MonoBehaviour
{
    [SerializeField] private MonsterData_Karin monsterData;
    [SerializeField] private int level = 3;
    [SerializeField] private GameObject alert; // Sprite tanda !

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

        // Logika sederhana: Jika pemain dekat (< 3.25 unit), munculkan tanda !
        if (distanceToPlayer < 3.25f)
        {
            if (alert != null) alert.SetActive(true);
            // Pindah ke mode mengejar (Chase)
        }
        else
        {
            if (alert != null) alert.SetActive(false);
            // Kembali ke mode santai (Wander / Return)
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inBattle = true;
            Debug.Log("Menyentuh pemain! Masuk ke Scn_Battle.");
            // Di sini nanti memicu perpindahan scene battle
        }
    }
}