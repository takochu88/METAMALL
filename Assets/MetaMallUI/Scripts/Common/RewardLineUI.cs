using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector.Editor.Internal;
using UnityEngine;

/// <summary>
/// 横一行分の RewardIconUI を並べるセル。
/// MyScrollRect のセルとして使う。LayoutGroup 不使用で手動配置。
/// 単独では使用しない。RewardListUI から情報を受け取る。
/// </summary>
public class RewardLineUI : MonoBehaviour
{
    private readonly List<RewardIconUI> icons = new();
    private RewardIconUI iconPrefab;
    private float spacing;
    private RectTransform rt;
    private float iconSize;
    private int lineIndex;
    private int totalInLine;

    private RectTransform Rect => rt ??= (RectTransform)transform;

    /// <summary>
    /// アイコンプレファブ・1行あたりのアイコン数・スペーシングを指定して初期化する。
    /// RewardListUI から呼ばれる。
    /// </summary>
    public void Setup(RewardIconUI iconPrefab, int countPerLine, float spacing)
    {
        this.iconPrefab = iconPrefab;
        this.spacing = spacing;
        EnsureIconCount(countPerLine);

        for (int i = 0; i < icons.Count; i++)
            icons[i].SetVisible(i < countPerLine);
    }

    /// <summary>アイコン数が足りなければ生成（既存の表示状態は変えない）。</summary>
    public void EnsureIconCount(int count)
    {
        for (int i = icons.Count; i < count; i++)
        {
            var icon = Instantiate(iconPrefab, transform);
            InitIconRect(icon);
            icon.SetVisible(false);
            icons.Add(icon);
        }
    }

    private static void InitIconRect(RewardIconUI icon)
    {
        var r = (RectTransform)icon.transform;
        r.anchorMin = new Vector2(0.5f, 0.5f);
        r.anchorMax = new Vector2(0.5f, 0.5f);
        r.pivot = new Vector2(0.5f, 0.5f);
    }

    /// <summary>行情報を設定する。</summary>
    public void SetLineInfo(int lineIndex, int totalInLine)
    {
        this.lineIndex = lineIndex;
        this.totalInLine = totalInLine;
    }

    /// <summary>アイコンサイズを設定。visibleCount 以降は非表示にする。</summary>
    public void SetIconSize(float size, int visibleCount = -1)
    {
        iconSize = size;
        for (int i = 0; i < icons.Count; i++)
        {
            if (visibleCount >= 0 && i >= visibleCount)
                icons[i].SetVisible(false);
            else
                icons[i].SetSize(size);
        }
    }

    /// <summary>行のデータを設定する。レイアウトは親が LayoutIcons で行う。</summary>
    public void SetData(List<RewardEntry> rewards, int startIndex, int count, bool canUse = false, bool dark = false)
    {
        for (int i = 0; i < icons.Count; i++)
        {
            int dataIndex = startIndex + i;
            if (i < count && dataIndex < rewards.Count)
            {
                var entry = rewards[dataIndex];
                icons[i].SetVisible(true);
                var iconDisplayType = canUse ? RewardIconDisplayType.StockAmount : RewardIconDisplayType.Amount;
                icons[i].SetData(entry.rewardId, iconDisplayType, entry.amount);
                icons[i].SetVisibleDark(dark);
            }
            else
            {
                icons[i].SetVisible(false);
            }
        }
    }

    /// <summary>
    /// アイコンの位置を再計算する。
    /// </summary>
    public void LayoutIcons(RewardListOneLineArrangeType arrangeType = RewardListOneLineArrangeType.Center, float padding = 0f)
    {
        float containerWidth = Rect.rect.width - padding * 2f;
        int layoutCount = lineIndex == 0 ? totalInLine : icons.Count;
        float totalWidth = layoutCount * iconSize + Mathf.Max(0, layoutCount - 1) * spacing;

        float startX;
        switch (arrangeType)
        {
            case RewardListOneLineArrangeType.Left:
                startX = -containerWidth * 0.5f;
                break;
            case RewardListOneLineArrangeType.Right:
                startX = containerWidth * 0.5f - totalWidth;
                break;
            default: // Center
                startX = -totalWidth * 0.5f;
                break;
        }

        for (int i = 0; i < icons.Count; i++)
        {
            float centerX = startX + iconSize * 0.5f + i * (iconSize + spacing);
            var r = (RectTransform)icons[i].transform;
            r.anchoredPosition = new Vector2(centerX, 0f);
        }
    }

    /// <summary>指定アイコンにデータを設定して出現演出を再生する。</summary>
    public void RevealIcon(int iconIndex, int rewardId, int amount, float duration, Ease ease)
    {
        var icon = GetIcon(iconIndex);
        if (icon == null) return;
        icon.SetSize(iconSize);
        icon.SetData(rewardId, RewardIconDisplayType.Amount, amount);
        icon.PrepareReveal();
        icon.PlayReveal(duration, ease);
    }

    /// <summary>アイコン取得。</summary>
    public RewardIconUI GetIcon(int index) => (index >= 0 && index < icons.Count) ? icons[index] : null;

    /// <summary>全アイコンを非表示にする。</summary>
    public void HideAllIcons()
    {
        foreach (var icon in icons) icon.SetVisible(false);
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActiveIfChanged(visible);
    }
    
    public void AllSetVisibleDark(bool visible)
    {
        for (int i = 0; i < icons.Count; i++)
        {
            int index = i;
            SetVisibleDark(index, visible);
        }
    }

    public void SetVisibleDark(int index, bool visible)
    {
        icons[index].SetVisible(visible);
    }
}
