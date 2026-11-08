public enum GrowthType 
{ 
    LevelUp, 
    Evolution 
}

public struct GrowthEvent
{
    public GrowthType Type;
    public MonsterDataI From;   // Data sebelum (bukan Instance!)
    public MonsterDataI To;     // Data sesudah (bukan Instance!)
    public string Message;
}