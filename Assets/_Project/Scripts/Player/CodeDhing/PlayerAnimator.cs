using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    // Menggunakan Hash agar lebih performa (tidak cari string tiap frame)
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");

    [SerializeField] private PlayerMovement movement;
    private Animator animator;

    void Awake() => animator = GetComponent<Animator>();

    void Update()
    {
        // Cek apakah player bergerak
        bool isMoving = movement.Velocity.sqrMagnitude > 0.01f;
        animator.SetBool(IsMoving, isMoving);

        // Kirim arah hadap ke Blend Tree
        animator.SetFloat(MoveX, movement.Facing.x);
        animator.SetFloat(MoveY, movement.Facing.y);
    }
}