using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerConfig config;
    [SerializeField] private Rigidbody2D rb;
    private AudioService audio;
    private float footstepTimer;
    private IMoveInput input;
    private Vector2 velocity;

    // Property untuk dibaca PlayerAnimator
    public Vector2 Velocity => velocity;
    public Vector2 Facing { get; private set; } = Vector2.down;

    public void Construct(IMoveInput moveInput) => input = moveInput;

    void Awake()
    {
        // ✅ Cari AudioService dari scene Bootstrap (DontDestroyOnLoad)
        audio = FindAnyObjectByType<AudioService>();
        if (audio == null)
            Debug.LogWarning("[PlayerMovement] AudioService tidak ditemukan! SFX footstep mati.");
    }

    void Update()
    {
        if (input == null) return;
        Vector2 move = input.Move;
        float speed = input.IsRunning ? config.RunSpeed : config.WalkSpeed;
        velocity = move * speed;
        
        if (move.sqrMagnitude > 0.01f)
        {
            Facing = ToCardinal(move);
            
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                footstepTimer = 0.35f;
                if (audio != null) audio.PlayFootstep();
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }
    void FixedUpdate() => rb.linearVelocity = velocity;

    // BARU: Mengubah input diagonal menjadi 4 arah utama
    private static Vector2 ToCardinal(Vector2 v) =>
        Mathf.Abs(v.x) > Mathf.Abs(v.y) 
            ? new Vector2(Mathf.Sign(v.x), 0f) 
            : new Vector2(0f, Mathf.Sign(v.y));

    public void Teleport(Vector2 position, Vector2 facing)
{
    transform.position = position;
    Facing = facing;
}
}