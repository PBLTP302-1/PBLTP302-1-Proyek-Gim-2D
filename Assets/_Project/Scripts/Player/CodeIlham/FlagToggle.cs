using UnityEngine;

public class FlagToggle : MonoBehaviour
{
    [Header("Konfigurasi")]
    [SerializeField] private string flagKey;      // Misal: "has_starter"
    [SerializeField] private GameObject hideWhenSet;  // Palang Gate_Hutan

    private StoryFlags flags;

    // Dipanggil oleh WorldInstaller
    public void Construct(StoryFlags flags)
    {
        this.flags = flags;
        
        // Cek kondisi awal
        if (flags != null && flags.Has(flagKey))
        {
            if (hideWhenSet != null) hideWhenSet.SetActive(false);
        }

        // Dengarkan perubahan flag
        if (flags != null)
        {
            flags.Changed += OnFlagChanged;
        }
    }

    private void OnFlagChanged(string key)
    {
        if (key == flagKey)
        {
            Debug.Log($"[FlagToggle] Flag '{flagKey}' aktif, menyembunyikan {hideWhenSet?.name}");
            if (hideWhenSet != null) hideWhenSet.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (flags != null)
        {
            flags.Changed -= OnFlagChanged;
        }
    }
}