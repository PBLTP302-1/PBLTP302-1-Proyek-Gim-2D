using UnityEngine;

public class FlagToggle : MonoBehaviour
{
    [SerializeField] private string flagKey;
    [SerializeField] private GameObject hideWhenSet;
    [SerializeField] private bool activeWhenSet;

    private StoryFlags flags;

    public void Construct(StoryFlags flags)
    {
        this.flags = flags;
        if (flags != null)
        {
            flags.Changed -= OnFlag;
            flags.Changed += OnFlag;
            Apply(flags.Has(flagKey));
        }
    }

    private void OnDestroy()
    {
        if (flags != null) flags.Changed -= OnFlag;
    }

    private void OnFlag(string key)
    {
        if (key == flagKey) Apply(flags.Has(flagKey));
    }

    private void Apply(bool has)
    {
        if (hideWhenSet == null) return;
        bool active = has ? activeWhenSet : !activeWhenSet;
        hideWhenSet.SetActive(active);
        Debug.Log($"[FlagToggle] {flagKey}={has} → {hideWhenSet.name} active={active}");
    }
}