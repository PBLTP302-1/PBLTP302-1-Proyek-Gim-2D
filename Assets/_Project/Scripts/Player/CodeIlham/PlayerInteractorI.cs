using UnityEngine;

public class PlayerInteractorI : MonoBehaviour
{
    [SerializeField] private PlayerMovementI movement;
    [SerializeField] private float reach = 0.6f;
    [SerializeField] private float radius = 0.3f;
    [SerializeField] private LayerMask interactableMask;

    private IActionInputI input;

    public void Construct(IActionInputI actionInput) => input = actionInput;

    private Vector2 ProbePoint => (Vector2)transform.position + movement.Facing * reach;

    void Update()
    {
        if (input == null)
        {
            Debug.LogWarning("[PlayerInteractor] input NULL! WorldInstaller belum Construct().");
            return;
        }

        if (!input.InteractPressed) return;

        Collider2D hit = Physics2D.OverlapCircle(ProbePoint, radius, interactableMask);

        if (hit != null && hit.TryGetComponent(out IInteractableI target))
        {
            Debug.Log($"[PlayerInteractor] Kena: {hit.name}");
            target.Interact(this);
        }
        else
        {
            Debug.LogWarning($"[PlayerInteractor] Tidak kena NPC. ProbePoint={ProbePoint}, radius={radius}, mask={interactableMask.value}");
        }
    }
}