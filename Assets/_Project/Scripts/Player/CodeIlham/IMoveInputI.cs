using UnityEngine;

public interface IMoveInputI
{
    Vector2 Move { get; }
    bool IsRunning { get; }
}