using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementI : MonoBehaviour
{
    [Header("Kecepatan")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float sprintMultiplier = 2f;

    private Rigidbody2D body;
    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.down;
    private InputGate inputGate;
    private float currentSpeed;

    public Vector2 Velocity => body != null ? body.linearVelocity : Vector2.zero;
    public Vector2 Facing => lastMoveDirection;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    public void Construct(InputGate gate)
    {
        inputGate = gate;
    }

    void Update()
    {
        // 🟢 CEK INPUTGATE - player tidak bisa bergerak saat battle/dialog
        if (inputGate != null && inputGate.IsLocked)
        {
            moveInput = Vector2.zero;
            return;
        }

        Keyboard k = Keyboard.current;
        Gamepad g = Gamepad.current;

        moveInput = Vector2.zero;

        if (k != null)
        {
            if (k.upArrowKey.isPressed || k.wKey.isPressed) moveInput.y = 1;
            if (k.downArrowKey.isPressed || k.sKey.isPressed) moveInput.y = -1;
            if (k.leftArrowKey.isPressed || k.aKey.isPressed) moveInput.x = -1;
            if (k.rightArrowKey.isPressed || k.dKey.isPressed) moveInput.x = 1;
        }

        if (g != null)
        {
            moveInput.x = g.leftStick.x.ReadValue();
            moveInput.y = g.leftStick.y.ReadValue();
        }

        // Logika Sprint (Shift)
        bool isSprinting = false;
        if (k != null)
        {
            isSprinting = k.leftShiftKey.isPressed || k.rightShiftKey.isPressed;
        }
        currentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;

        if (moveInput != Vector2.zero)
        {
            lastMoveDirection = moveInput.normalized;
        }

        moveInput.Normalize();
    }

    void FixedUpdate()
    {
        if (body != null)
        {
            body.linearVelocity = moveInput * currentSpeed;
        }
    }
}