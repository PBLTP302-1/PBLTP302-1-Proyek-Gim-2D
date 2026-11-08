using System.Collections;
using UnityEngine;

public class NpcI : MonoBehaviour, IInteractableI
{
    [Header("Data NPC")]
    [SerializeField] protected string npcName = "Warga";
    [SerializeField] protected SpriteRenderer body;
    [SerializeField] protected Sprite faceDown, faceLeft, faceRight, faceUp;
    [SerializeField] protected DialogueData dialogue;

    // 🟢 Property yang bisa diakses oleh class turunan
    protected DialogueRunner Dialogue { get; private set; }
    protected StoryFlags Flags { get; private set; }
    protected PlayerPartyI Party { get; private set; }
    protected ToastView Toast { get; private set; } // 🟢 BARU

    public virtual void Construct(DialogueRunner dialogue, StoryFlags flags, PlayerPartyI party, ToastView toast)
    {
        Dialogue = dialogue;
        Flags = flags;
        Party = party;
        Toast = toast; // 🟢 BARU
        OnConstructed();
    }

    public void Interact(PlayerInteractorI who)
    {
        if (Dialogue == null)
        {
            Debug.LogWarning($"[NpcI] Dialogue NULL pada {npcName}!");
            Face(who.transform.position - transform.position);
            return;
                }

        if (Dialogue.IsRunning) return;

        Face(who.transform.position - transform.position);
        Dialogue.Run(Talk());
    }

    protected void Face(Vector2 d)
    {
        if (body == null) return;
        body.sprite = Mathf.Abs(d.x) > Mathf.Abs(d.y)
            ? (d.x > 0 ? faceRight : faceLeft)
            : (d.y > 0 ? faceUp : faceDown);
    }

    protected virtual IEnumerator Talk()
    {
        if (dialogue != null)
        {
            yield return Dialogue.PlayData(dialogue);
        }
    }

    protected virtual void OnConstructed() { }
}