using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private float reach = 0.6f;   // Jarak jangkauan
    [SerializeField] private float radius = 0.3f;  // Radius deteksi
    [SerializeField] private LayerMask interactableMask;

    private IActionInput input;

    public void Construct(IActionInput actionInput) => input = actionInput;

    // Titik di depan player berdasarkan arah hadap
    private Vector2 ProbePoint => (Vector2)transform.position + movement.Facing * reach;

    void Update()
    {
        if (input == null || !input.InteractPressed) return;

        // Cari collider dalam radius di depan player
        Collider2D hit = Physics2D.OverlapCircle(ProbePoint, radius, interactableMask);
        
        if (hit != null && hit.TryGetComponent(out IInteractable target))
        {
            target.Interact(this);
        }
    }

}
