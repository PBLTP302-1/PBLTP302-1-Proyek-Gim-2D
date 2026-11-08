using UnityEngine;

public class StateMachineI
{
    public IStateI Current { get; private set; }

    public void ChangeTo(IStateI next) {
        if (next == Current) return;
        Current?.Exit();
        Current = next;
        Current.Enter();
    }

    public void Tick(float deltaTime) => Current?.Tick(deltaTime);
}