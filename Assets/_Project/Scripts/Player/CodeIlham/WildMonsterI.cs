using System.Collections;
using UnityEngine;

public class WildMonsterI : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float chaseRadius = 4f;
    [SerializeField] private float stopRadius = 0.5f;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float returnSpeed = 2.5f;
    [SerializeField] private MonsterDataI data;
    [SerializeField] private int level = 3;
    [SerializeField] private bool isBoss = false;

    [Header("Alert Indicator")]
    [SerializeField] private GameObject alertIndicator;
    [SerializeField] private float alertBobSpeed = 3f;
    [SerializeField] private float alertBobHeight = 0.15f;

    private Rigidbody2D rb;
    private Collider2D monsterCollider;
    private Transform player;
    private SceneFlowI scenes;
    private BattleSessionI session;
    private InputGate gate;
    
    private Vector3 originalHomePosition;
    private Vector3 battleStartPosition;
    private Vector3 alertOriginalPos;
    
    private bool isChasing = false;
    private bool battleStarted = false;
    private bool isReturningHome = false;
    private float alertTimer = 0f;
    private float cooldown = 0f;
    
    private const float CooldownSeconds = 4f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        monsterCollider = GetComponent<Collider2D>();
        originalHomePosition = transform.position;
        battleStartPosition = originalHomePosition;
        
        if (alertIndicator != null)
        {
            alertIndicator.SetActive(false);
            alertOriginalPos = alertIndicator.transform.localPosition;
        }
    }

    public void Construct(BattleSessionI session, SceneFlowI scenes, 
                          Transform playerTransform, InputGate gate = null)
    {
        this.session = session;
        this.scenes = scenes;
        player = playerTransform;
        this.gate = gate;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !battleStarted && monsterCollider != null && monsterCollider.isTrigger)
        {
            if (session != null && session.IsPlayerSafe()) return;
            if (cooldown > 0f) return;
            if (session != null && session.IsBattleActiveOrLoading) return;
            
            StartBattle();
        }
    }

    void StartBattle()
    {
        if (battleStarted) return;
        battleStarted = true;

        StopChase();
        isReturningHome = false;
        battleStartPosition = transform.position;

        if (alertIndicator != null) alertIndicator.SetActive(false);

        if (session != null)
        {
            session.SetBattleLoading(true);
            session.SetEnemy(new MonsterInstanceI(data, level), isBoss, this);
        }
        
        gameObject.SetActive(false);
        
        if (scenes != null) scenes.LoadBattle();
    }

    void Update()
    {
        // 🟢 DEKLARASI isPlayerSafe HANYA SEKALI DI SINI
        bool isPlayerSafe = session != null && session.IsPlayerSafe();

        // 1. TOGGLE COLLIDER (PALING ATAS)
        if (monsterCollider != null)
        {
            bool shouldBeTrigger = !isPlayerSafe;
            if (monsterCollider.isTrigger != shouldBeTrigger)
            {
                monsterCollider.isTrigger = shouldBeTrigger;
            }
        }

        // 2. Handle Cooldown (Freeze)
        if (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
            if (isChasing) StopChase();
            isReturningHome = false;
            return;
        }

        if (player == null || battleStarted) return;

        // Handle InputGate (Dialog/Battle lain)
        if (gate != null && gate.IsLocked)
        {
            if (isChasing) StopChase();
            isReturningHome = false;
            return;
        }

        // 3. Cek Waktu Aman Global
        if (isPlayerSafe)
        {
            if (isChasing) StopChase();
            isReturningHome = false;
            return;
        }

        // 4. Logika Pergerakan Normal (Chase, Re-engage, Return Home)
        float dist = Vector3.Distance(transform.position, player.position);
        float distToHome = Vector3.Distance(transform.position, originalHomePosition);

        if (dist < chaseRadius && dist > stopRadius)
        {
            if (isReturningHome) isReturningHome = false;
            if (!isChasing) StartChase();
            
            Vector3 direction = (player.position - transform.position).normalized;
            Vector3 newPos = transform.position + direction * moveSpeed * Time.deltaTime;
            
            if (rb != null) rb.MovePosition(newPos);
            else transform.position = newPos;
        }
        else if (dist <= stopRadius)
        {
            StartBattle();
        }
        else
        {
            if (isChasing) StopChase();

            if (distToHome > 0.1f)
            {
                isReturningHome = true;
                Vector3 direction = (originalHomePosition - transform.position).normalized;
                Vector3 newPos = transform.position + direction * returnSpeed * Time.deltaTime;

                if (Vector3.Distance(newPos, originalHomePosition) < returnSpeed * Time.deltaTime)
                {
                    newPos = originalHomePosition;
                    isReturningHome = false;
                }

                if (rb != null) rb.MovePosition(newPos);
                else transform.position = newPos;
            }
            else
            {
                isReturningHome = false;
            }
        }

        // Animasi Alert Indicator (Bobbing)
        if (alertIndicator != null && alertIndicator.activeSelf)
        {
            alertTimer += Time.deltaTime;
            float bobY = Mathf.Sin(alertTimer * alertBobSpeed) * alertBobHeight;
            alertIndicator.transform.localPosition = alertOriginalPos + new Vector3(0, bobY, 0);
        }
    }

    private void StartChase()
    {
        isChasing = true;
        if (alertIndicator != null)
        {
            alertIndicator.SetActive(true);
            alertTimer = 0f;
        }
    }

    private void StopChase()
    {
        isChasing = false;
        if (alertIndicator != null)
        {
            alertIndicator.SetActive(false);
            alertIndicator.transform.localPosition = alertOriginalPos;
        }
    }

    public void OnBattleEnd(BattleResultI result)
    {
         Debug.Log($"[WildMonster] {gameObject.name}: OnBattleEnd({result})");
        
        battleStarted = false;
        StopChase();
        isReturningHome = false;

        if (result == BattleResultI.Win)
        {
            // Menang: Monster hilang, respawn 10 detik
            StartCoroutine(RespawnWithCooldown(10f, CooldownSeconds));
        }
        else if (result == BattleResultI.Flee)
        {
            // Kabur: Monster tetap di posisi saat battle + cooldown 4 detik
            gameObject.SetActive(true);
            cooldown = CooldownSeconds;
        }
        else if (result == BattleResultI.Lose)
        {
            // 🟢 KALAH: Monster langsung di-teleport ke posisi awal (seperti reset scene)
            // Kita panggil ForceResetToHome() langsung di sini karena monster ini sedang non-aktif
            // dan tidak terdeteksi oleh FindObjectsByType di TeleportToSaveStone.
            ForceResetToHome();
        }
    }

    private IEnumerator RespawnWithCooldown(float respawnDelay, float cooldownDelay)
    {
        yield return new WaitForSeconds(respawnDelay);
        
        gameObject.SetActive(true);
        battleStartPosition = originalHomePosition;
        transform.position = originalHomePosition;
        
        cooldown = cooldownDelay;
    }
     public void ForceResetToHome()
    {
        Debug.Log($"[WildMonster] {gameObject.name}: Force reset to home position.");
        
        // Reset semua state
        battleStarted = false;
        StopChase();
        isReturningHome = false;
        cooldown = 0f; // Hapus cooldown agar monster langsung aktif kembali
        
        // Pastikan monster muncul (jika sebelumnya di-hide saat battle)
        gameObject.SetActive(true);
        
        // Kembalikan ke posisi spawn asli
        transform.position = originalHomePosition;
        battleStartPosition = originalHomePosition;
    }
}