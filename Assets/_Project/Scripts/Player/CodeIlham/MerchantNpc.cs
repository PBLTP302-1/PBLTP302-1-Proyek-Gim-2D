using System.Collections;
using UnityEngine;

public class MerchantNpc : NpcI
{
    [Header("Merchant NPC")]
    [SerializeField] private int giftPotions = 3;

    protected override IEnumerator Talk()
    {
        if (Flags.Has(StoryFlags.GotGift))
        {
            yield return Dialogue.Say(npcName, "Kamu sudah mendapat hadiah dariku. Rawat partnermu baik-baik!");
        }
        else
        {
            yield return Dialogue.Say(npcName, "Halo! Aku punya hadiah untukmu.");
            yield return Dialogue.Say(npcName, $"Ini {giftPotions} Ramuan. Gunakan saat HP partnermu rendah.");

            // 🟢 Gunakan Party dari parent NpcI (bukan field sendiri)
            Party.AddPotions(giftPotions);
            Flags.Set(StoryFlags.GotGift);

            yield return Dialogue.Say("", $"Kamu mendapat {giftPotions} Ramuan!");
        }
    }

    // 🟢 TIDAK PERLU override Construct() - biarkan parent NpcI yang handle
    // 🟢 TIDAK PERLU field Party sendiri - gunakan Party dari parent
}