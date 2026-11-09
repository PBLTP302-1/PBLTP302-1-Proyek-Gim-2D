using UnityEngine;

public class FlagToggle : MonoBehaviour
{
    [Header("Konfigurasi")]
    [SerializeField] private string flagKey;
    [SerializeField] private GameObject hideWhenSet;

    private StoryFlags flags;

    public void Construct(StoryFlags flags)
    {
        this.flags = flags;
        
        // Cek kondisi awal (jika flag sudah aktif dari save)
        if (flags != null && flags.Has(flagKey))
        {
            Debug.Log($"[FlagToggle] Flag '{flagKey}' sudah aktif, menyembunyikan {hideWhenSet?.name}");
            if (hideWhenSet != null) hideWhenSet.SetActive(false);
        }

        // Dengarkan perubahan flag
        if (flags != null)
        {
            flags.Changed += OnFlagChanged;
            Debug.Log($"[FlagToggle] Mendaftarkan listener untuk flag: {flagKey}");
        }
    }

    private void OnFlagChanged(string key)
    {
        if (key == flagKey)
        {
            Debug.Log($"[FlagToggle] Flag '{flagKey}' baru saja di-set! Menyembunyikan {hideWhenSet?.name}");
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