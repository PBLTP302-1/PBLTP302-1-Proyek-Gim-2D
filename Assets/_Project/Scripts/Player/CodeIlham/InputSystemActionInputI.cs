using UnityEngine.InputSystem;
public class InputSystemActionInputI : IActionInputI
{
    private readonly InputAction interact;
    public InputSystemActionInputI()
    {
        interact = InputSystem.actions.FindAction("Interact");
    }
    public bool InteractPressed => interact.WasPressedThisFrame();
}

