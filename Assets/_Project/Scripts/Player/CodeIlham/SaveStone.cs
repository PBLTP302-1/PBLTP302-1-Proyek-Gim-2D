using System.Collections;
using UnityEngine;

public class SaveStone : MonoBehaviour, IInteractableI
{
    private GameServicesI services;
    private PlayerMovementI player;

    // Dipanggil oleh WorldInstaller
    public void Construct(GameServicesI s, PlayerMovementI p)
    {
        services = s;
        player = p;
    }

    public void Interact(PlayerInteractorI who)
    {
        // Hanya bisa berinteraksi jika tidak ada dialog yang sedang berjalan
        if (services.Dialogue != null && !services.Dialogue.IsRunning)
        {
            services.Dialogue.Run(Use());
        }
    }

    private IEnumerator Use()
    {
        // 1. Pulihkan HP Partner sepenuhnya
        if (services.Party.Partner != null)
        {
            // Gunakan HealFull() jika ada, atau Heal(9999) sebagai alternatif aman
            services.Party.Partner.Heal(9999); 
        }

        // 2. Ambil data saat ini dan simpan ke JSON
        bool saved = services.Save.Save(SaveMapper.Capture(services, player.transform.position));

        // 3. Berikan feedback melalui dialog
        string message = saved 
            ? "Batu Simpan bersinar. HP partner pulih dan perjalanan tersimpan." 
            : "Batu Simpan bersinar. HP partner pulih, tetapi penyimpanan gagal.";

        yield return services.Dialogue.Say("", message);
    }
}