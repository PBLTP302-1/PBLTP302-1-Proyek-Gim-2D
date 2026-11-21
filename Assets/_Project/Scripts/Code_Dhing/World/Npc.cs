using System.Collections;
using UnityEngine;

public class Npc : MonoBehaviour, IInteractable
{
    [SerializeField] protected string npcName = "Warga";
    [SerializeField] protected SpriteRenderer body;
    [SerializeField] protected Sprite faceDown, faceLeft, faceRight, faceUp;
    [SerializeField] protected DialogueData dialogue;

    protected DialogueRunner Dialogue { get; private set; }
    protected ToastView Toast { get; private set; }
    protected StoryFlags Flags { get; private set; }
    protected string NpcName => npcName;

    public void Construct(DialogueRunner dialogue, StoryFlags flags, ToastView toast)
    {
        Dialogue = dialogue;
        Flags = flags;
        Toast = toast;
        OnConstructed();
    }

    public void Construct(DialogueRunner dialogue, StoryFlags flags)
    {
        Dialogue = dialogue;
        Flags = flags;
        OnConstructed();
    }

    public void Interact(PlayerInteractor who)
    {
        if (Dialogue != null && Dialogue.IsRunning) return;
        Face(who.transform.position - transform.position);
        if (Dialogue != null)
            Dialogue.Run(Talk());
    }

    protected virtual IEnumerator Talk()
    {
        if (dialogue != null && Dialogue != null)
            yield return Dialogue.PlayData(dialogue);
    }

    protected virtual void OnConstructed() { }

    protected void Face(Vector2 d)
    {
        if (body == null) return;
        body.sprite = Mathf.Abs(d.x) > Mathf.Abs(d.y)
            ? (d.x > 0 ? faceRight : faceLeft)
            : (d.y > 0 ? faceUp : faceDown);
    }
}