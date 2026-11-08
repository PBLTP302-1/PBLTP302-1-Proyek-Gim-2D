using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Input untuk menu: panah, confirm, cancel.
/// Support keyboard dan gamepad.
/// </summary>
public static class MenuInput
{
    static Keyboard K => Keyboard.current;
    static Gamepad G => Gamepad.current;

    public static bool Up =>
        (K != null && (K.upArrowKey.wasPressedThisFrame || K.wKey.wasPressedThisFrame)) ||
        (G != null && G.dpad.up.wasPressedThisFrame);

    public static bool Down =>
        (K != null && (K.downArrowKey.wasPressedThisFrame || K.sKey.wasPressedThisFrame)) ||
        (G != null && G.dpad.down.wasPressedThisFrame);

    public static bool Left =>
        (K != null && (K.leftArrowKey.wasPressedThisFrame || K.aKey.wasPressedThisFrame)) ||
        (G != null && G.dpad.left.wasPressedThisFrame);

    public static bool Right =>
        (K != null && (K.rightArrowKey.wasPressedThisFrame || K.dKey.wasPressedThisFrame)) ||
        (G != null && G.dpad.right.wasPressedThisFrame);

    public static bool Confirm =>
        (K != null && (K.eKey.wasPressedThisFrame || K.spaceKey.wasPressedThisFrame ||
                       K.enterKey.wasPressedThisFrame || K.numpadEnterKey.wasPressedThisFrame ||
                       K.zKey.wasPressedThisFrame)) ||
        (G != null && G.buttonSouth.wasPressedThisFrame);

    public static bool Cancel =>
        (K != null && (K.escapeKey.wasPressedThisFrame || K.xKey.wasPressedThisFrame ||
                       K.backspaceKey.wasPressedThisFrame)) ||
        (G != null && G.buttonEast.wasPressedThisFrame);

    public static bool Mute =>
        K != null && K.mKey.wasPressedThisFrame;
}