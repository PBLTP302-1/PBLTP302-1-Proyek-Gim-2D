using System.Collections;
using UnityEngine;

public class GuardNpc : NpcI
{
    [Header("Posisi Menyamping")]
    [SerializeField] private Vector2 asidePosition = new Vector2(22.5f, 10.4f);

    protected override void OnConstructed()
    {
        // 1. Cek kondisi awal (Penting jika player melakukan Load Game)
        // Jika player sudah punya partner, langsung geser penjaganya
        if (Flags.Has(StoryFlags.HasStarter)) 
        {
            transform.position = asidePosition;
        }
        
        // 2. Dengarkan perubahan flag saat game sedang berjalan
        Flags.Changed += OnFlagChanged;
    }

    private void OnFlagChanged(string key)
    {
        // Jika flag has_starter baru saja diaktifkan oleh Tetua, geser ke samping
        if (key == StoryFlags.HasStarter) 
        {
            Debug.Log($"[GuardNpc] Flag has_starter aktif! Penjaga bergeser ke {asidePosition}");
            transform.position = asidePosition;
        }
    }

    protected override IEnumerator Talk()
    {
        if (!Flags.Has(StoryFlags.HasStarter))
        {
            // Dialog jika player BELUM punya partner
            yield return Dialogue.Say(npcName, "Berhenti! Hutan terlalu berbahaya tanpa monster partner. Temui Tetua dulu.");
        }
        else
        {
            // Dialog jika player SUDAH punya partner
            yield return Dialogue.Say(npcName, "Jalan terbuka. Monster di rumput tinggi akan mengejarmu kalau terlalu dekat.");
        }
    }

    // Hapus listener saat objek dihancurkan untuk mencegah memory leak
    void OnDestroy()
    {
        Flags.Changed -= OnFlagChanged;
    }
}