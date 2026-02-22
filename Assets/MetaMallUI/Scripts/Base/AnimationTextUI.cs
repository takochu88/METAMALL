using PrimeTween;
using TMPro;
using UnityEngine;

/// <summary>
/// 数値の増減をアニメーション表示する汎用コンポーネント。
/// カウント演出（旧値→新値へ段階的に変化）と差分ポップアップ（+150 / -50）を行う。
/// </summary>
public class AnimationTextUI : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private TMP_Text mainText;
    [SerializeField] private TMP_Text diffText;
    [SerializeField] private CanvasGroup diffCanvasGroup;

    [Header("カウント演出")]
    [SerializeField] private float countDuration = 0.5f;
    [SerializeField] private Ease countEase = Ease.OutCubic;

    [Header("差分ポップアップ")]
    [SerializeField] private float diffDuration = 1.0f;
    [SerializeField] private float diffMoveY = 60f;
    [SerializeField] private Color increaseColor = Color.green;
    [SerializeField] private Color decreaseColor = Color.red;

    public RectTransform rect;
    private int currentValue;
    private Tween countTween;
    private Sequence diffSequence;
    private RectTransform diffRect;
    private Vector2 diffBasePos;

    private void Awake()
    {
        diffRect = diffText.GetComponent<RectTransform>();
        diffBasePos = diffRect.anchoredPosition;
        diffCanvasGroup.alpha = 0f;
    }

    /// <summary>値を設定する。animate=true でカウント演出、showDiff=true で差分ポップアップを再生。</summary>
    public void SetValue(int value, bool animate = true, bool showDiff = true)
    {
        if (!animate)
        {
            SetValueImmediate(value);
            return;
        }

        int from = currentValue;
        int diff = value - from;
        currentValue = value;

        if (diff == 0)
        {
            mainText.text = value.ToString();
            return;
        }

        PlayCount(from, value);
        if (showDiff) PlayDiff(diff);
    }

    /// <summary>アニメなしで即座に値を反映する。</summary>
    public void SetValueImmediate(int value)
    {
        countTween.Stop();
        diffSequence.Stop();
        currentValue = value;
        mainText.text = value.ToString();
        diffCanvasGroup.alpha = 0f;
    }

    private void PlayCount(int from, int to)
    {
        countTween.Stop();
        countTween = Tween.Custom(this, from, to, countDuration, (self, val) =>
        {
            self.mainText.text = ((int)val).ToString();
        }, countEase);

        // 完了時バウンス（Scale 1→1.1→1）
        Tween.Scale(mainText.transform, 1.1f, countDuration * 0.15f, Ease.OutQuad)
            .OnComplete(mainText.transform, t => Tween.Scale(t, 1f, 0.1f, Ease.InQuad));
    }

    private void PlayDiff(int diff)
    {
        diffSequence.Stop();

        // テキスト設定
        diffText.text = diff > 0 ? $"+{diff}" : diff.ToString();
        diffText.color = diff > 0 ? increaseColor : decreaseColor;

        // 開始位置リセット
        diffRect.anchoredPosition = diffBasePos;
        diffCanvasGroup.alpha = 1f;

        // 移動 + フェードアウトを並列実行
        diffSequence = Sequence.Create()
            .Group(Tween.UIAnchoredPosition(diffRect, diffBasePos + new Vector2(0f, diffMoveY), diffDuration, Ease.OutQuad))
            .Group(Tween.Alpha(diffCanvasGroup, 0f, diffDuration * 0.6f, Ease.Linear, startDelay: diffDuration * 0.4f));
    }

    private void OnDisable()
    {
        countTween.Stop();
        diffSequence.Stop();
    }
}
