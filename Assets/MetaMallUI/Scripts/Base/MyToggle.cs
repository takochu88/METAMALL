using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// MyToggleGroup で管理される個別トグル。
/// アイコン + テキスト、選択時の色 / Tween 演出に対応。
/// </summary>
public class MyToggle : MonoBehaviour, IPointerClickHandler
{
    // ── 表示要素 ─────────────────────────────
    [BoxGroup("表示")]
    [SerializeField] Image icon;
    [BoxGroup("表示")]
    [SerializeField] TMP_Text label;
    [BoxGroup("表示")]
    [SerializeField] Image background;

    // ── 色設定 ───────────────────────────────
    [BoxGroup("色")]
    [SerializeField] Color normalColor = Color.white;
    [BoxGroup("色")]
    [SerializeField] Color selectedColor = new Color(0.2f, 0.6f, 1f);
    [BoxGroup("色")]
    [SerializeField] Color normalTextColor = Color.white;
    [BoxGroup("色")]
    [SerializeField] Color selectedTextColor = Color.white;

    // ── Tween ───────────────────────────────
    [BoxGroup("Tween")]
    [SerializeField] bool useTween = true;
    [BoxGroup("Tween")]
    [ShowIf("useTween")]
    [SerializeField] float tweenDuration = 0.15f;
    [BoxGroup("Tween")]
    [ShowIf("useTween")]
    [SerializeField] float selectedScale = 1.1f;
    [BoxGroup("Tween")]
    [ShowIf("useTween")]
    [SerializeField] Ease tweenEase = Ease.OutQuad;

    // ── 内部 ─────────────────────────────────
    MyToggleGroup group;
    bool isOn;
    Tween scaleTween;
    Tween bgColorTween;
    Tween textColorTween;

    // ── プロパティ ───────────────────────────
    public int Index { get; internal set; }
    public bool IsOn => isOn;

    // ── 初期化 ───────────────────────────────
    internal void Init(MyToggleGroup owner, int index)
    {
        group = owner;
        Index = index;
    }

    public void SetContent(Sprite iconSprite, string text)
    {
        if (icon != null) icon.sprite = iconSprite;
        if (label != null) label.text = text;
    }

    // ── 選択状態の切替 ──────────────────────
    internal void SetSelected(bool selected, bool instant)
    {
        isOn = selected;

        if (!useTween || instant)
        {
            ApplyImmediate();
            return;
        }

        ApplyAnimated();
    }

    void ApplyImmediate()
    {
        scaleTween.Stop();
        bgColorTween.Stop();
        textColorTween.Stop();

        transform.localScale = isOn ? Vector3.one * selectedScale : Vector3.one;

        if (background != null)
            background.color = isOn ? selectedColor : normalColor;

        if (label != null)
            label.color = isOn ? selectedTextColor : normalTextColor;
    }

    void ApplyAnimated()
    {
        var targetScale = isOn ? Vector3.one * selectedScale : Vector3.one;
        scaleTween.Stop();
        scaleTween = Tween.Scale(transform, targetScale, tweenDuration, tweenEase);

        if (background != null)
        {
            var targetColor = isOn ? selectedColor : normalColor;
            bgColorTween.Stop();
            bgColorTween = Tween.Color(background, targetColor, tweenDuration, tweenEase);
        }

        if (label != null)
        {
            var targetColor = isOn ? selectedTextColor : normalTextColor;
            textColorTween.Stop();
            textColorTween = Tween.Color(label, targetColor, tweenDuration, tweenEase);
        }
    }

    // ── タップ ──────────────────────────────
    public void OnPointerClick(PointerEventData eventData)
    {
        group?.Select(Index);
    }

    // ── ライフサイクル ──────────────────────
    void OnDisable()
    {
        scaleTween.Stop();
        bgColorTween.Stop();
        textColorTween.Stop();
        transform.localScale = Vector3.one;
    }
}
