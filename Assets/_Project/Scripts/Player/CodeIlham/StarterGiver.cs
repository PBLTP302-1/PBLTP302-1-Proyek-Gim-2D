using System.Collections;
using UnityEngine;

public class StarterGiver : NpcI
{
    [Header("Starter Giver")]
    [SerializeField] private MonsterDataI[] starters;
    [SerializeField] private int starterLevel = 3;
    // 🟢 HAPUS: [SerializeField] private ToastView toast; (sudah diwariskan dari NpcI)

    protected override IEnumerator Talk()
    {
        yield return Dialogue.Say(npcName, "Ah, kamu datang juga! Hutan di utara penuh monster liar.");
        yield return Dialogue.Say(npcName, "Tidak ada yang boleh masuk hutan tanpa partner. Pilih satu dari tiga ini.");

        MonsterDataI pick = null;
        while (pick == null)
        {
            int choice = 0, sure = 0;

            string[] options = new string[starters.Length];
            for (int i = 0; i < starters.Length; i++)
                options[i] = starters[i].DisplayName;

            yield return Dialogue.Choose(npcName, "Siapa yang akan menjadi partnermu?", options, c => choice = c);

            MonsterDataI m = starters[choice];

            yield return Dialogue.Choose(npcName, $"{m.DisplayName} tipe {m.Element}. Yakin?",
                new string[] { "Ya", "Pilih lagi" }, c => sure = c);

            if (sure == 0) pick = m;
        }

        if (Party != null)
        {
            Party.SetPartner(new MonsterInstanceI(pick, starterLevel));
        }
        else
        {
            Debug.LogError("[StarterGiver] Party null! Partner tidak bisa di-set.");
        }

        if (Flags != null) Flags.Set(StoryFlags.HasStarter);

        yield return Dialogue.Say("", $"Kamu mendapat {pick.DisplayName}!");
        yield return Dialogue.Say(npcName, "Rawat dia baik-baik. Menang bertarung membuatnya naik level, lalu berevolusi.");
        yield return Dialogue.Say(npcName, "Batu Simpan di sebelahku menyimpan perjalananmu dan memulihkan partnermu.");

        // 🟢 GUNAKAN Toast yang diwariskan dari NpcI
        if (Toast != null)
            Toast.Show("Penjaga Batas sudah membuka jalan ke hutan");
    }
}