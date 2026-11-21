using System.Collections;
using UnityEngine;

public class SariNpc : Npc
{
    private string[] tips = new string[]
    {
        "Pyro kuat melawan Sprout! Api membakar tumbuhan.",
        "Sprout kuat melawan Aqua! Akar menyerap air.",
        "Aqua kuat melawan Pyro! Air memadamkan api.",
        "Volt kuat melawan Aqua! Listrik menghantam air.",
        "Kalau tipe lawan lebih kuat, serangannya jadi 'Kurang efektif...'",
        "Coba pakai Jurus (Skill) — damage-nya lebih besar dari Serang biasa!",
        "Kabur dari monster liar biasanya berhasil. Tapi dari boss? Mustahil!"
    };

    protected override IEnumerator Talk()
    {
        int index = Random.Range(0, tips.Length);
        yield return Dialogue.Say(NpcName, tips[index]);

        yield return Dialogue.Say(NpcName, "Semoga berhasil, petualang!");
    }
}