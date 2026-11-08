using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorI : MonoBehaviour
{
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    [SerializeField] private PlayerMovementI movement;
    private Animator animator;
    void Awake() => animator = GetComponent<Animator>();
    void Update()
{
    bool isMoving = movement.Velocity.sqrMagnitude > 0.01f;
    float moveX = movement.Facing.x;
    float moveY = movement.Facing.y;
    
    animator.SetBool(IsMoving, isMoving);
    animator.SetFloat(MoveX, moveX);
    animator.SetFloat(MoveY, moveY);
    
    
}
}
