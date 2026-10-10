using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerConfig config;
    [SerializeField] private Rigidbody2D rb;
    private IMoveInput input;
    private Vector2 velocity;
    public void Construct(IMoveInput moveInput) => input = moveInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
    if (input == null) return;
    float speed = input.IsRunning ? config.RunSpeed :
    config.WalkSpeed;
    velocity = input.Move * speed;
    }
    void FixedUpdate() => rb.linearVelocity = velocity;
}
