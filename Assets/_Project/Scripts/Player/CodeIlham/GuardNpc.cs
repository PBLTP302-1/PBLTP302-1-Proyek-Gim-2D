using System.Collections;
using UnityEngine;

public class GuardNpc : NpcI
{
    [Header("Posisi Menyamping")]
    [SerializeField] private Vector2 asidePosition = new Vector2(22.5f, 10.4f);

    protected override void OnConstructed()
    {
        // 1. Cek kondisi awal (Penting untuk Load Game)
        // Jika player sudah punya partner dari save file, langsung geser
        if (Flags.Has(StoryFlags.HasStarter)) 
        {
            transform.position = asidePosition;
        }
        
        // 2. Dengarkan perubahan flag saat bermain
        Flags.Changed += OnFlagChanged;
    }

    private void OnFlagChanged(string key)
    {
        // Jika flag has_starter baru saja diaktifkan, geser ke samping
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
            yield return Dialogue.Say(npcName, "Berhenti! Hutan terlalu berbahaya tanpa monster partner. Temui Tetua dulu.");
        }
        else
        {
            yield return Dialogue.Say(npcName, "Jalan terbuka. Monster di rumput tinggi akan mengejarmu kalau terlalu dekat.");
        }
    }

    // Hapus listener saat objek dihancurkan untuk mencegah memory leak
    void OnDestroy()
    {
        Flags.Changed -= OnFlagChanged;
    }
}