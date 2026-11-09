using System.Collections;
using UnityEngine;

public class MerchantNpc : NpcI
{
    [Header("Konfigurasi Hadiah")]
    [SerializeField] private int giftPotions = 3;

    protected override IEnumerator Talk()
    {
        // Cek apakah player sudah pernah menerima hadiah
        if (Flags.Has(StoryFlags.GotGift))
        {
            // Dialog jika sudah pernah dapat
            yield return Dialogue.Say(npcName, "Kamu sudah mendapat hadiah dariku. Rawat partnermu baik-baik!");
            yield return Dialogue.Say(npcName, "Batu Simpan di sebelah Tetua bisa memulihkan HP partner dan menyimpan perjalanan.");
        }
        else
        {
            // Dialog pertama kali
            yield return Dialogue.Say(npcName, "Halo! Aku punya hadiah untukmu.");
            yield return Dialogue.Say(npcName, $"Ini {giftPotions} Ramuan. Gunakan saat HP partnermu rendah.");

            // Berikan Ramuan ke Party
            if (Party != null)
            {
                Party.AddPotions(giftPotions);
                Debug.Log($"[MerchantNpc] Player mendapat {giftPotions} Ramuan. Total: {Party.Potions}");
            }

            // Set flag agar tidak bisa diminta lagi
            Flags.Set(StoryFlags.GotGift);

            yield return Dialogue.Say("", $"Kamu mendapat {giftPotions} Ramuan!");
            yield return Dialogue.Say(npcName, "Gunakan dengan bijak. Selamat berpetualang!");
        }
    }
}