using UnityEngine;

[CreateAssetMenu(fileName = "Mon_", menuName = "Pulau Monster/Monster Data")]
public class MonsterDataI : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private Element element;           // 🟢 WAJIB ADA
    [SerializeField] private Sprite[] frames;
    [SerializeField] private int baseHP, baseATK, baseDEF, baseSPD;
    [SerializeField] private string skillName = "Semburan Api";  // 🟢 WAJIB ADA
    [SerializeField] private MonsterDataI evolvesTo;
    [SerializeField] private int evolveLevel;

    public string Id => id;
    public string DisplayName => displayName;
    public Element Element => element;                  // 🟢 PROPERTY BARU
    public Sprite[] Frames => frames;
    public int BaseHP => baseHP;
    public int BaseATK => baseATK;
    public int BaseDEF => baseDEF;
    public int BaseSPD => baseSPD;
    public string SkillName => skillName;               // 🟢 PROPERTY BARU
    public MonsterDataI EvolvesTo => evolvesTo;
    public int EvolveLevel => evolveLevel;
}