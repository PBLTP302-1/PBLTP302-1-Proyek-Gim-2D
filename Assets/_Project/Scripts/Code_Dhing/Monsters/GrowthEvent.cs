using UnityEngine;

public enum GrowthType { LevelUp, Evolved }

public readonly struct GrowthEvent
{
    public readonly GrowthType Type;
    public readonly string Message;
    public readonly MonsterData From;
    public readonly MonsterData To;

    public GrowthEvent(GrowthType type, string message, MonsterData from = null, MonsterData to = null)
    {
        Type = type;
        Message = message;
        From = from;
        To = to;
    }
}
