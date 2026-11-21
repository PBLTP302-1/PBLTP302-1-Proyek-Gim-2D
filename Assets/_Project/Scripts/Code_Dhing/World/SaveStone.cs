using System.Collections;
using UnityEngine;

public class SaveStone : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private PixelBurst burst;
    private GameServices services;

    public void Construct(GameServices services) => this.services = services;

    public void Interact(PlayerInteractor who)
    {
        if (services != null && !services.Dialogue.IsRunning)
            services.Dialogue.Run(Use());
    }

    private IEnumerator Use()
    {
        if (burst != null && services.Party.Partner != null)
        {
            burst.Play(player.transform.position + Vector3.up * 0.5f,
                       new Color32(110, 231, 242, 255), 16, 50f, 0.7f);
        }
        
        if (services.Audio != null) services.Audio.PlaySave();
        if (services.Party.Partner != null)
            services.Party.Partner.HealFull();
        
        bool saved = services.Save.Save(SaveMapper.Capture(services, player.transform.position));
        
        yield return services.Dialogue.Say("", saved
            ? "Batu Simpan bersinar. HP partner pulih dan perjalanan tersimpan."
            : "Batu Simpan bersinar. HP partner pulih, tetapi penyimpanan gagal.");
    }
}