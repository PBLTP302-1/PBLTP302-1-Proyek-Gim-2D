using System.Collections;
using UnityEngine;

public class SaveStone : MonoBehaviour, IInteractableI
{
    private GameServicesI services;
    private PlayerMovementI player;

    // 🟢 FALLBACK: Jika services.Save null, buat sendiri
    private SaveService localSave;

    void Awake()
    {
        Debug.Log($"[SaveStone] Awake: {gameObject.name}");
    }

    public void Construct(GameServicesI s, PlayerMovementI p)
    {
        services = s;
        player = p;

        Debug.Log($"[SaveStone] Construct: services={s != null}, player={p != null}");

        // 🟢 FALLBACK 1: Jika services.Save null, buat instance lokal
        if (s != null && s.Save == null)
        {
            Debug.LogWarning("[SaveStone] services.Save NULL! Membuat SaveService lokal sebagai fallback.");
            localSave = new SaveService();
        }

        // 🟢 FALLBACK 2: Jika player null, cari di scene
        if (p == null)
        {
            Debug.LogWarning("[SaveStone] player NULL! Mencari PlayerMovementI di scene...");
            player = FindAnyObjectByType<PlayerMovementI>();
            if (player != null)
                Debug.Log($"[SaveStone] Player ditemukan: {player.gameObject.name}");
            else
                Debug.LogError("[SaveStone] Player TIDAK DITEMUKAN di scene!");
        }
    }

    public void Interact(PlayerInteractorI who)
    {
        Debug.Log("[SaveStone] Interact dipanggil!");

        if (services == null)
        {
            Debug.LogError("[SaveStone] services NULL total! Tidak bisa proceed.");
            return;
        }

        if (services.Dialogue != null && !services.Dialogue.IsRunning)
        {
            services.Dialogue.Run(Use());
        }
        else
        {
            Debug.LogWarning("[SaveStone] Dialogue NULL atau sedang berjalan.");
        }
    }

    private IEnumerator Use()
    {
        // 1. Heal Partner
        if (services.Party != null && services.Party.Partner != null)
        {
            services.Party.Partner.Heal(9999);
            Debug.Log("[SaveStone] Partner di-heal.");
        }

        // 2. Tentukan SaveService yang akan dipakai (prioritas services.Save, fallback localSave)
        SaveService saveToUse = services.Save ?? localSave;

        if (saveToUse == null)
        {
            Debug.LogError("[SaveStone] Tidak ada SaveService sama sekali!");
            yield return services.Dialogue.Say("", "Batu Simpan bersinar, tetapi sistem penyimpanan rusak.");
            yield break;
        }

        // 3. Tentukan player yang akan dipakai
        if (player == null)
        {
            Debug.LogError("[SaveStone] player masih NULL setelah fallback!");
            yield return services.Dialogue.Say("", "Batu Simpan bersinar, tetapi posisi player tidak ditemukan.");
            yield break;
        }

        // 4. Simpan Game
        Debug.Log($"[SaveStone] Menyimpan dengan SaveService: {saveToUse.GetType().Name}, posisi: {player.transform.position}");
        bool saved = saveToUse.Save(SaveMapper.Capture(services, player.transform.position));

        // 5. Dialog Feedback
        string message = saved
            ? "Batu Simpan bersinar. HP partner pulih dan perjalanan tersimpan."
            : "Batu Simpan bersinar. HP partner pulih, tetapi penyimpanan gagal.";

        yield return services.Dialogue.Say("", message);
    }
}