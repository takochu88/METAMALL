using UnityEngine;

public enum Dimension
{
    D3, // 第1次元
    D4, // 第2次元
    D5, // 第3次元
    D6, // 第4次元
    D7, // 第5次元
}

public static class DimensionExtensions
{
    private static readonly string[] Labels = { "D3", "D4", "D5", "D6", "D7" };

    private static readonly Color[] Colors =
    {
        new Color(1f,    1f,    1f,    1f), // D3: 白
        new Color(0.3f,  0.5f,  1f,    1f), // D4: 青
        new Color(1f,    0.84f, 0f,    1f), // D5: 黄
        new Color(0.9f,  0.2f,  0.2f,  1f), // D6: 赤
        new Color(0.7f,  0.3f,  0.9f,  1f), // D7: 紫
    };

    public static string Name(this Dimension type) => Labels[(int)type];

    public static Color Color(this Dimension type) => Colors[(int)type];
}
