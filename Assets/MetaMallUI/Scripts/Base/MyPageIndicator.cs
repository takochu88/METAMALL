using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ページインディケーター（○○○●○○ 形式）。
/// 表示上限を超える場合は iOS 風に端のドットを段階的に小さくする。
/// </summary>
public class MyPageIndicator : MonoBehaviour
{
    // ── ドット ──────────────────────────────────
    [BoxGroup("ドット")]
    [SerializeField] Sprite dotSprite;
    [BoxGroup("ドット")]
    [SerializeField] float dotSize = 8f;
    [BoxGroup("ドット")]
    [SerializeField] float dotSpacing = 12f;

    // ── 表示 ────────────────────────────────────
    [BoxGroup("表示")]
    [Tooltip("最大表示ドット数（奇数推奨）")]
    [SerializeField] int maxVisible = 5;
    [BoxGroup("表示")]
    [SerializeField] Color currentColor = Color.white;
    [BoxGroup("表示")]
    [SerializeField] Color normalColor = new Color(1f, 1f, 1f, 0.3f);

    // ── エッジ ──────────────────────────────────
    [BoxGroup("エッジ")]
    [Tooltip("端から1番目のスケール")]
    [SerializeField] float edgeScale1 = 0.6f;
    [BoxGroup("エッジ")]
    [Tooltip("端から2番目（最端）のスケール")]
    [SerializeField] float edgeScale2 = 0.3f;

    // ── Tween ───────────────────────────────────
    [BoxGroup("Tween")]
    [SerializeField] float tweenDuration = 0.2f;
    [BoxGroup("Tween")]
    [SerializeField] Ease tweenEase = Ease.OutQuad;

    // ── 内部 ────────────────────────────────────
    readonly List<RectTransform> dots = new();
    readonly List<Tween> scaleTweens = new();
    readonly List<Tween> colorTweens = new();
    int currentIndex;
    int totalCount;
    bool initialized;

    // ── プロパティ ──────────────────────────────
    public int CurrentIndex => currentIndex;
    public int TotalCount => totalCount;

    // ── Public API ─────────────────────────────

    /// <summary>アニメーション付きでページを設定する。</summary>
    public void SetPage(int currentIndex, int totalCount)
    {
        Apply(currentIndex, totalCount, instant: !initialized);
        initialized = true;
    }

    /// <summary>即時反映でページを設定する。</summary>
    public void SetPageImmediate(int currentIndex, int totalCount)
    {
        Apply(currentIndex, totalCount, instant: true);
        initialized = true;
    }

    // ── コア ────────────────────────────────────

    void Apply(int newIndex, int newTotal, bool instant)
    {
        newTotal = Mathf.Max(newTotal, 0);
        newIndex = newTotal > 0 ? Mathf.Clamp(newIndex, 0, newTotal - 1) : 0;

        currentIndex = newIndex;
        totalCount = newTotal;

        EnsureDots();

        // totalCount <= 1 のときは全ドット非表示
        bool visible = totalCount > 1;
        int visibleCount = visible ? Mathf.Min(totalCount, maxVisible) : 0;

        // ウィンドウ開始インデックスを計算
        int windowStart = 0;
        if (visible && totalCount > maxVisible)
        {
            int half = maxVisible / 2;
            windowStart = currentIndex - half;
            windowStart = Mathf.Clamp(windowStart, 0, totalCount - maxVisible);
        }

        // 全ドットの位置・状態を更新
        float totalWidth = (visibleCount - 1) * dotSpacing;
        float startX = -totalWidth * 0.5f;

        for (int i = 0; i < dots.Count; i++)
        {
            if (i >= visibleCount)
            {
                dots[i].gameObject.SetActiveIfChanged(false);
                continue;
            }

            var dot = dots[i];
            dot.gameObject.SetActiveIfChanged(true);

            // 位置
            dot.anchoredPosition = new Vector2(startX + i * dotSpacing, 0f);

            // ページインデックス
            int pageIndex = windowStart + i;
            bool isCurrent = pageIndex == currentIndex;

            // スケール計算
            float scale = CalcScale(i, visibleCount, windowStart);
            var targetScale = Vector3.one * scale;
            var targetColor = isCurrent ? currentColor : normalColor;

            var img = dot.GetComponent<Image>();

            if (instant)
            {
                StopTween(i);
                dot.localScale = targetScale;
                img.color = targetColor;
            }
            else
            {
                ApplyAnimated(i, dot, img, targetScale, targetColor);
            }
        }
    }

    float CalcScale(int dotIndex, int visibleCount, int windowStart)
    {
        if (totalCount <= maxVisible) return 1f;

        bool hasLeftEdge = windowStart > 0;
        bool hasRightEdge = windowStart + maxVisible < totalCount;

        // 左端チェック
        if (hasLeftEdge)
        {
            if (dotIndex == 0) return edgeScale2;
            if (dotIndex == 1) return edgeScale1;
        }

        // 右端チェック
        if (hasRightEdge)
        {
            if (dotIndex == visibleCount - 1) return edgeScale2;
            if (dotIndex == visibleCount - 2) return edgeScale1;
        }

        return 1f;
    }

    // ── アニメーション ─────────────────────────

    void ApplyAnimated(int i, Transform dot, Image img, Vector3 targetScale, Color targetColor)
    {
        StopTween(i);
        scaleTweens[i] = Tween.Scale(dot, targetScale, tweenDuration, tweenEase);
        colorTweens[i] = Tween.Color(img, targetColor, tweenDuration, tweenEase);
    }

    void StopTween(int i)
    {
        if (i < scaleTweens.Count) scaleTweens[i].Stop();
        if (i < colorTweens.Count) colorTweens[i].Stop();
    }

    void StopAllTweens()
    {
        for (int i = 0; i < scaleTweens.Count; i++) scaleTweens[i].Stop();
        for (int i = 0; i < colorTweens.Count; i++) colorTweens[i].Stop();
    }

    // ── ドット生成 ──────────────────────────────

    void EnsureDots()
    {
        int needed = Mathf.Min(totalCount, maxVisible);

        while (dots.Count < needed)
        {
            var dot = CreateDot(dots.Count);
            dots.Add(dot);
            scaleTweens.Add(default);
            colorTweens.Add(default);
        }
    }

    RectTransform CreateDot(int index)
    {
        var go = new GameObject($"Dot_{index}", typeof(RectTransform), typeof(Image));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(transform, false);
        rt.sizeDelta = new Vector2(dotSize, dotSize);
        rt.anchoredPosition = Vector2.zero;

        var img = go.GetComponent<Image>();
        img.sprite = dotSprite;
        img.color = normalColor;
        img.raycastTarget = false;

        return rt;
    }

    // ── ライフサイクル ──────────────────────────

    void OnDisable()
    {
        StopAllTweens();
        for (int i = 0; i < dots.Count; i++)
            dots[i].localScale = Vector3.one;
    }

#if UNITY_EDITOR
    // ── デバッグ ────────────────────────────────
    [BoxGroup("デバッグ")]
    [SerializeField, PropertyOrder(100)]
    int debugTotalCount = 10;

    [BoxGroup("デバッグ")]
    [SerializeField, PropertyOrder(100)]
    int debugCurrentIndex;

    [BoxGroup("デバッグ"), ButtonGroup("デバッグ/btn"), PropertyOrder(101)]
    [Button("SetPage (アニメ)")]
    void DebugSetPage() => SetPage(debugCurrentIndex, debugTotalCount);

    [BoxGroup("デバッグ"), ButtonGroup("デバッグ/btn"), PropertyOrder(101)]
    [Button("SetPage (即座)")]
    void DebugSetPageImmediate() => SetPageImmediate(debugCurrentIndex, debugTotalCount);
#endif
}
