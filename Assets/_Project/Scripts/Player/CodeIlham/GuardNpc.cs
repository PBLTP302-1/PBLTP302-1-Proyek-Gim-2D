using System.Collections;
using UnityEngine;

public class GuardNpc : NpcI
{
    [Header("Guard NPC")]
    [SerializeField] private Vector2 asidePosition = new Vector2(22.5f, 10.4f);

    protected override void OnConstructed()
    {
        Flags.Changed += OnFlagChanged;
        if (Flags.Has(StoryFlags.HasStarter)) StepAside();
    }

    private void OnFlagChanged(string key)
    {
        if (key == StoryFlags.HasStarter) StepAside();
    }

    private void StepAside() => transform.position = asidePosition;

    protected override IEnumerator Talk()
    {
        if (!Flags.Has(StoryFlags.HasStarter))
            yield return Dialogue.Say(npcName, "Berhenti! Hutan terlalu berbahaya tanpa monster partner. Temui Tetua dulu.");
        else
            yield return Dialogue.Say(npcName, "Jalan terbuka. Monster di rumput tinggi akan mengejarmu kalau terlalu dekat.");
    }
}