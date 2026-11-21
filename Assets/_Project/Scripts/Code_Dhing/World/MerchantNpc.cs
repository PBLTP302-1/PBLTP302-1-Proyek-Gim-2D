using System.Collections;
using UnityEngine;

public class MerchantNpc : Npc
{
    private PlayerParty party;

    public void SetParty(PlayerParty party) => this.party = party;
    protected override IEnumerator Talk()
    {
        if (Flags.Has(StoryFlags.MerchantGift))
        {
            yield return Dialogue.Say(NpcName, "Sudah kuberi hadiah tadi. Semangat bertualang!");
            yield break;
        }

        yield return Dialogue.Say(NpcName, "Halo, petualang muda! Aku punya hadiah untukmu.");
        yield return Dialogue.Say(NpcName, "Ini 3 Ramuan. Gunakan saat HP partnermu rendah ya!");

        if (party != null)
        {
            party.Potions += 3;
            Flags.Set(StoryFlags.MerchantGift);
            if (Toast != null)
                Toast.Show("Mendapat 3 Ramuan!");
        }

        yield return Dialogue.Say(NpcName, "Sampai jumpa!");
    }
}