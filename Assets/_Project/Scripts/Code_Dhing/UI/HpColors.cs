using UnityEngine;

public static class HpColors
{
    public static Color For(float ratio)
    {
        if (ratio > 0.5f) return Color.green;
        if (ratio > 0.2f) return UiTheme.Gold;
        return Color.red;
    }
    public static Color High => new Color32(110, 231, 242, 255);
}