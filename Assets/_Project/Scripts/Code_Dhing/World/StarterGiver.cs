using System.Collections;
using UnityEngine;

public class StarterGiver : Npc
{
    [SerializeField] private MonsterData[] starters;
    [SerializeField] private int starterLevel = 3;
    private PlayerParty party;

    public void SetParty(PlayerParty party) => this.party = party;
    
    protected override IEnumerator Talk()
    {
        if (Flags.Has(StoryFlags.HasStarter))
        {
            yield return Dialogue.Say(NpcName, "Partnermu sudah hebat! Rawat dia baik-baik.");
            yield break;
        }

        yield return Dialogue.Say(NpcName, "Ah, kamu datang juga! Hutan di utara penuh monster liar.");
        yield return Dialogue.Say(NpcName, "Tidak ada yang boleh masuk hutan tanpa partner. Pilih satu dari tiga ini.");

        string[] options = new string[starters.Length];
        for (int i = 0; i < starters.Length; i++)
            options[i] = starters[i].DisplayName;

        MonsterData pick = null;
        while (pick == null)
        {
            int choice = 0, sure = 0;
            yield return Dialogue.Choose(NpcName, "Siapa yang akan menjadi partnermu?", options, c => choice = c);
            MonsterData m = starters[choice];
            yield return Dialogue.Choose(NpcName, $"{m.DisplayName} tipe {m.Element}. Yakin?",
                new[] { "Ya", "Pilih lagi" }, c => sure = c);
            if (sure == 0) pick = m;
        }

        party.SetPartner(new MonsterInstance(pick, starterLevel));
        Flags.Set(StoryFlags.HasStarter);
        yield return Dialogue.Say("", $"Kamu mendapat {pick.DisplayName}!");
        yield return Dialogue.Say(NpcName, "Rawat dia baik-baik. Menang bertarung membuatnya naik level, lalu berevolusi.");
        yield return Dialogue.Say(NpcName, "Batu Simpan di sebelahku menyimpan perjalananmu dan memulihkan partnermu.");

        if (Toast != null)
            Toast.Show("Penjaga Batas sudah membuka jalan ke hutan");
    }

    public static Color ElementColor(Element elem)
{
    switch (elem)
    {
        case Element.Pyro: return new Color32(255, 100, 50, 255);
        case Element.Aqua: return new Color32(50, 150, 255, 255);
        case Element.Sprout: return new Color32(100, 200, 50, 255);
        case Element.Volt: return new Color32(255, 220, 50, 255);
        default: return Color.white;
    }
}
}
