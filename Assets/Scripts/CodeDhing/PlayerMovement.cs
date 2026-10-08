using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Rigidbody2D rb;
    private InputAction moveAction;
    private Vector2 moveInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() => moveAction =
            moveAction = InputSystem.actions.FindAction("Move");

    // Update is called once per frame
    void Update() => moveInput = moveAction.ReadValue<Vector2>();

    void FixedUpdate() => rb.linearVelocity = moveInput * moveSpeed;
}
