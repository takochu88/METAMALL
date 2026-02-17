using UnityEngine;

public static class RectTransformExtensions
{
    /// <summary>
    /// gaugeArea の左端(0)〜右端(1) に target を水平移動させる。
    /// pivot を考慮するので、左寄せ・中央寄せどちらでも正しく動く。
    /// </summary>
    public static void SetGaugePosition(this RectTransform target, RectTransform gaugeArea, float t)
    {
        t = Mathf.Clamp01(t);
        var size = gaugeArea.rect.size;
        var pivot = gaugeArea.pivot;

        var pos = target.anchoredPosition;
        pos.x = Mathf.Lerp(-size.x * pivot.x, size.x * (1f - pivot.x), t);
        target.anchoredPosition = pos;
    }
}
