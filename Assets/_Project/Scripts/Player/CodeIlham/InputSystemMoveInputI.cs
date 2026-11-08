using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemMoveInputI : IMoveInputI
{
    private readonly InputAction move;
    private readonly InputAction sprint;
    public InputSystemMoveInputI()
    {
        move = InputSystem.actions.FindAction("Move");
        sprint = InputSystem.actions.FindAction("Sprint");
    }
    public Vector2 Move => move.ReadValue<Vector2>();
    public bool IsRunning => sprint != null && sprint.IsPressed();
}
