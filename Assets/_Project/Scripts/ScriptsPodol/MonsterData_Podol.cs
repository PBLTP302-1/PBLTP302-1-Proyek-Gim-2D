using UnityEngine;

[CreateAssetMenu(fileName = "Mon_", menuName = "Pulau Monster/Monster Data")]
public class MonsterData_Podol : ScriptableObject
{

    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private Element_Podol element;
    [SerializeField] private Sprite[] frames; // Idle, Attack, Hurt
    [SerializeField] private int baseHP, baseATK, baseDEF, baseSPD;
    [SerializeField] private string skillName = "Semburan Api";
    [SerializeField] private MonsterData_Podol evolvesTo;
    [SerializeField] private int evolveLevel;

    public string Id => id;
    public string DisplayName => displayName;
    public Element_Podol Element => element;
    public Sprite[] Frames => frames;
    public int BaseHP => baseHP;
    public int BaseATK => baseATK;
    public int BaseDEF => baseDEF;
    public int BaseSPD => baseSPD;
    public string SkillName => skillName;
    public MonsterData_Podol EvolvesTo => evolvesTo;
    public int EvolveLevel => evolveLevel;
}

public enum Element_Podol { Pyro, Aqua, Sprout, Volt }