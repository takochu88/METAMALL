using System;
using PrimeTween;
using UnityEngine;

public class MenuUIBase : MonoBehaviour
{
    // ── 設定 ─────────────────────────────────
    [SerializeField] private RectTransform rect;

    // ── 内部 ─────────────────────────────────
    Tween currentTween;

    // ── プロパティ ───────────────────────────
    public RectTransform Rect => rect;

    // ── 表示制御 ─────────────────────────────
    public void SetVisible(bool visible)
    {
        gameObject.SetActiveIfChanged(visible);
    }

    /// <summary>
    /// スライドインで表示する。
    /// </summary>
    /// <param name="fromRight">true=右から登場, false=左から登場</param>
    /// <param name="onComplete">アニメーション完了時コールバック</param>
    public void SlideIn(bool fromRight, float duration, Ease ease, Action onComplete = null)
    {
        currentTween.Stop();
        gameObject.SetActiveIfChanged(true);

        float width = rect.rect.width;
        // 右から入る → 開始位置は右(+)  左から入る → 開始位置は左(-)
        float startX = fromRight ? width : -width;

        rect.anchoredPosition = new Vector2(startX, rect.anchoredPosition.y);
        currentTween = Tween.UIAnchoredPositionX(rect, 0f, duration, ease);
        if (onComplete != null)
            currentTween.OnComplete(onComplete);
    }

    /// <summary>
    /// スライドアウトで非表示にする。
    /// moveRight=true のとき、新UIが右から来るので旧UIは左へ退場。
    /// </summary>
    /// <param name="moveRight">true=左へ退場, false=右へ退場</param>
    public void SlideOut(bool moveRight, float duration, Ease ease)
    {
        currentTween.Stop();

        float width = rect.rect.width;
        // 新UIが右から来る(moveRight) → 旧UIは左(-) へ退場
        // 新UIが左から来る(!moveRight) → 旧UIは右(+) へ退場
        float endX = moveRight ? -width : width;

        currentTween = Tween.UIAnchoredPositionX(rect, endX, duration, ease)
            .OnComplete(this, self => self.gameObject.SetActiveIfChanged(false));
    }

    /// <summary>アニメなしで即座に表示位置をリセットする。</summary>
    public void ShowImmediate()
    {
        currentTween.Stop();
        gameObject.SetActiveIfChanged(true);
        rect.anchoredPosition = new Vector2(0f, rect.anchoredPosition.y);
    }

    /// <summary>アニメなしで即座に非表示にする。</summary>
    public void HideImmediate()
    {
        currentTween.Stop();
        gameObject.SetActiveIfChanged(false);
    }

    void OnDisable()
    {
        currentTween.Stop();
    }
}
