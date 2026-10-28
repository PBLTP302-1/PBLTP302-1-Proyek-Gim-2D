using UnityEngine.InputSystem;

public class InputSystemActionInput : IActionInput
{
    private readonly InputAction interact;

    public InputSystemActionInput()
    {
        interact = InputSystem.actions.FindAction("Interact"); // Pastikan ada action "Interact" (tombol E) di InputSystem_Actions
    }

    public bool InteractPressed => interact != null && interact.WasPressedThisFrame();

}
