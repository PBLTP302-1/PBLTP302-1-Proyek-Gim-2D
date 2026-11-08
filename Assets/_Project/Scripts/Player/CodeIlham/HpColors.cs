using UnityEngine;

public static class HpColors
{
    // Hijau > 50%, Emas > 20%, Merah selebihnya
    public static Color For(float ratio)
    {
        if (ratio > 0.5f) return new Color(0.3f, 0.85f, 0.35f, 1f);    // Hijau
        if (ratio > 0.2f) return new Color(1.0f, 0.77f, 0.1f, 1f);     // Emas
        return new Color(0.95f, 0.2f, 0.2f, 1f);                       // Merah
    }
}