using UnityEngine;

public class InputGate : IMoveInputI, IActionInputI
{
    private readonly IMoveInputI moveSource;
    private readonly IActionInputI actionSource;
    private bool locked = false;

    public InputGate(IMoveInputI move, IActionInputI action)
    {
        moveSource = move;
        actionSource = action;
    }

    // ====== IMoveInput ======
    public Vector2 Move => locked ? Vector2.zero : moveSource.Move;
    public bool IsRunning => locked ? false : moveSource.IsRunning;

    // ====== IActionInput ======
    public bool InteractPressed => locked ? false : actionSource.InteractPressed;

    // ====== Kontrol ======
    public bool IsLocked => locked;

    public void Lock()
    {
        locked = true;
    }

    public void Unlock()
    {
        locked = false;
    }
}
