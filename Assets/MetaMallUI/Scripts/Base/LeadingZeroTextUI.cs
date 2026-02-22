using TMPro;
using UnityEngine;

/// <summary>
/// 最大値の桁数に合わせてゼロ埋めし、先頭ゼロを薄く表示する汎用テキストUI。
/// 例: maxValue=900, value=30 → "<alpha=#33>0</alpha><alpha=#FF>30"
/// </summary>
public class LeadingZeroTextUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private byte leadingZeroAlpha = 0x33;

    private int digitCount = 1;

    /// <summary>最大値から表示桁数を決定する。</summary>
    public void SetMaxValue(int maxValue)
    {
        digitCount = maxValue <= 0 ? 1 : CountDigits(maxValue);
    }

    /// <summary>値を表示する。事前に SetMaxValue で桁数を設定しておくこと。</summary>
    public void SetValue(int value)
    {
        text.text = BuildText(value, digitCount, leadingZeroAlpha);
    }

    /// <summary>最大値と現在値を同時に設定する。</summary>
    public void SetValue(int value, int maxValue)
    {
        SetMaxValue(maxValue);
        SetValue(value);
    }

    private static string BuildText(int value, int digits, byte zeroAlpha)
    {
        string numStr = value.ToString();

        // 桁数が足りない分だけ先頭ゼロを追加
        int leadingCount = digits - numStr.Length;
        if (leadingCount <= 0) return numStr;

        string alphaHex = zeroAlpha.ToString("X2");
        return $"<alpha=#{alphaHex}>{new string('0', leadingCount)}<alpha=#FF>{numStr}";
    }

    private static int CountDigits(int value)
    {
        if (value < 0) value = -value;
        int count = 0;
        do
        {
            count++;
            value /= 10;
        } while (value > 0);
        return count;
    }

    public void SetiVisible(bool visible)
    {
        text.enabled = visible;
    }
    
    public float GetFontSize()
    {
        return text.fontSize;
    }

    public void SetFontSize(float fontSize)
    {
        text.fontSize = fontSize;
    }
}
