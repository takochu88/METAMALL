using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

/// <summary>リワードの表示モード</summary>
public enum RewardDisplayMode
{
    /// <summary>同じアイテムをまとめて表示する</summary>
    [LabelText("同じアイテムをまとめる")]
    AllAtOnce,

    /// <summary>同じアイテムもまとめずに1つずつ表示する</summary>
    [LabelText("同じアイテムをまとめない")]
    OneByOne,
}

/// <summary>同一IDリワードのまとめ方</summary>
public enum RewardMergeMode
{
    /// <summary>表示前に同一IDをまとめてから表示する</summary>
    [LabelText("表示前にまとめる")]
    PreMerge,

    /// <summary>まとめずにそのまま表示する</summary>
    [LabelText("まとめない")]
    NoMerge,

    /// <summary>演出後にまとめボタンを表示する</summary>
    [LabelText("演出後にまとめる")]
    PostMerge,
}

/// <summary>
/// RewardLineUI を使った縦スクロールのアイテム一覧。
/// ライン数に応じて自身の縦幅を調整する。
/// </summary>
public class RewardListUI : MonoBehaviour
{
    [BoxGroup("参照")]
    [SerializeField] private Image brockOverlay;

    // ── 設定 ─────────────────────────────────────
    [BoxGroup("設定")]
    [SerializeField] private MyScrollRect scrollRect;

    [BoxGroup("設定")]
    [LabelText("アイコン配置方法")]
    [Tooltip("アイコンをどのように並べるか（折り返し・行数指定など）")]
    [SerializeField] private RewardListArrangeType arrangeType = RewardListArrangeType.WrapByParentSize;

    [BoxGroup("設定")]
    [Tooltip("1行あたりのアイコン数")]
    [ShowIf("arrangeType", RewardListArrangeType.WrapByItemsPerRow)]
    [SerializeField] private int countPerLine = 4;

    [BoxGroup("設定")]
    [LabelText("1行時の揃え")]
    [Tooltip("アイテムが1行に収まる場合の揃え方向")]
    [SerializeField] private RewardListOneLineArrangeType oneLineArrangeType = RewardListOneLineArrangeType.Center;

    [BoxGroup("設定")]
    [LabelText("アイコンサイズ")]
    [Tooltip("各アイコンの幅と高さ（Setup で指定しなければこの値を使用）")]
    [SerializeField] private float iconSize = 100f;

    [BoxGroup("設定")]
    [LabelText("アイコン間隔")]
    [Tooltip("アイコン同士の横方向の間隔")]
    [SerializeField] private float iconSpacing = 10f;

    [BoxGroup("設定")]
    [LabelText("ラインの左右パディング")]
    [Tooltip("各ラインの左右に加える余白")]
    [SerializeField] private float linePadding = 0f;

    [BoxGroup("設定")]
    [LabelText("ラインごとの隙間")]
    [Tooltip("ライン同士の縦方向の間隔")]
    [SerializeField] private float lineSpacing = 0f;

    [BoxGroup("設定")]
    [LabelText("親との上下の隙間")]
    [Tooltip("親との上下の余白。高さ自動調整時にこの値を超えない")]
    [SerializeField] private float verticalMargin = 100f;

    [BoxGroup("設定")]
    [LabelText("最初と最後のラインの隙間")]
    [Tooltip("最初のラインの上端・最後のラインの下端に加える余白")]
    [SerializeField] private float heightPadding = 20f;

    [BoxGroup("設定")]
    [LabelText("高さ自動調整")]
    [Tooltip("true ならアイテム数に応じてリストの縦幅を自動調整する")]
    [SerializeField] private bool adjustHeight = true;

    // ── 表示 ─────────────────────────────────────
    [BoxGroup("表示")]
    [LabelText("表示モード")]
    [Tooltip("AllAtOnce: 全アイテムを一括表示 / OneByOne: 1つずつ順番に出現")]
    [SerializeField] private RewardDisplayMode displayMode = RewardDisplayMode.OneByOne;
    [BoxGroup("表示")]
    [LabelText("マージモード")]
    [Tooltip("PreMerge: 表示前に同一IDをまとめる / NoMerge: まとめない / PostMerge: 演出後にまとめボタンを表示")]
    [SerializeField] private RewardMergeMode mergeMode = RewardMergeMode.PreMerge;

    // ── アニメ ───────────────────────────────────
    [BoxGroup("アニメ")]
    [LabelText("アイコン毎の出現間隔")]
    [Tooltip("OneByOne 時のアイコン出現間隔（秒）")]
    [SerializeField] private float itemInterval = 0.08f;
    [BoxGroup("アニメ")]
    [LabelText("アイコン毎のアニメーション時間")]
    [Tooltip("各アイコンの出現アニメーションの長さ（秒）")]
    [SerializeField] private float revealDuration = 0.25f;
    [BoxGroup("アニメ")]
    [LabelText("イージング")]
    [Tooltip("出現アニメーションのイージング曲線")]
    [SerializeField] private Ease revealEase = Ease.OutBack;

    [BoxGroup("参照")]
    [SerializeField] private RewardIconUI iconPrefab;
    [BoxGroup("参照")]
    [SerializeField] private Button skipButton;
    [BoxGroup("参照")]
    [SerializeField] private Button mergeButton;
    [BoxGroup("参照")]
    [SerializeField] private RectTransform rectTransform;
    [BoxGroup("参照")]
    [SerializeField] private RectTransform parentRect;
    [BoxGroup("参照")]
    public Image background;
    // ── 内部 ─────────────────────────────────────
    private List<RewardEntry> rawRewards;
    private List<RewardEntry> displayRewards;
    private int lineCount;
    private int revealedCount;
    private Coroutine animCoroutine;
    private bool canUse;
    private int resolvedCountPerLine;
    private Action onComplete;
    
    void Awake()
    {
        if (skipButton != null) skipButton.onClick.AddListener(Skip);
        if (mergeButton != null) mergeButton.onClick.AddListener(Merge);
        PreGenerate();
    }

    // ══════════════════════════════════════════════
    //  事前生成
    // ══════════════════════════════════════════════

    /// <summary>
    /// 表示に必要な最大数のラインとアイコンを事前生成する。
    /// Setup 前に呼ぶとインスタンス生成の負荷を分散できる。
    /// </summary>
    public void PreGenerate()
    {
        Canvas.ForceUpdateCanvases();

        // 1行あたりのアイコン数を決定
        if (arrangeType == RewardListArrangeType.WrapByItemsPerRow)
        {
            resolvedCountPerLine = Mathf.Max(1, countPerLine);
        }
        else
        {
            var sr = scrollRect.GetComponent<ScrollRect>();
            var vp = sr.viewport != null ? sr.viewport : (RectTransform)sr.transform;
            float availableWidth = vp.rect.width - linePadding * 2f;
            resolvedCountPerLine = (availableWidth > 0f && iconSize > 0f)
                ? Mathf.Max(1, Mathf.FloorToInt((availableWidth + iconSpacing) / (iconSize + iconSpacing)))
                : 1;
        }

        scrollRect.SetCellMainSize(iconSize + lineSpacing);
        scrollRect.SetPadding(heightPadding, heightPadding);
        float stride = Mathf.Max(1f, iconSize + lineSpacing);
        int maxLines;

        if (arrangeType == RewardListArrangeType.SingleRowInfiniteScroll)
        {
            // 1行モードなら1ラインのみ
            maxLines = 1;
        }
        else if (adjustHeight)
        {
            // 高さ自動調整ON → 親の高さからマージンを引いた範囲に収まる最大ライン数
            float maxHeight = parentRect != null
                ? parentRect.rect.height - verticalMargin * 2f
                : rectTransform.rect.height;
            maxLines = Mathf.Max(1, Mathf.CeilToInt(maxHeight / stride)) + 3;
        }
        else
        {
            // 高さ自動調整OFF → 自身の高さに収まるライン数
            float viewportHeight = rectTransform.rect.height;
            maxLines = Mathf.Max(1, Mathf.CeilToInt(viewportHeight / stride)) + 3;
        }

        // 各セルに resolvedCountPerLine 個のアイコンも事前生成する
        scrollRect.Init(maxLines,
            cell => cell.Get<RewardLineUI>().Setup(iconPrefab, resolvedCountPerLine, iconSpacing),
            (index, cell) => { });
    }

    // ══════════════════════════════════════════════
    //  1行あたりのアイコン数を解決
    // ══════════════════════════════════════════════

    /// <summary>
    /// arrangeType に応じて1行あたりのアイコン数を算出する。
    /// </summary>
    int ResolveCountPerLine()
    {
        switch (arrangeType)
        {
            case RewardListArrangeType.SingleRowInfiniteScroll:
                return displayRewards != null ? Mathf.Max(1, displayRewards.Count) : 1;

            case RewardListArrangeType.WrapByItemsPerRow:
                return Mathf.Max(1, countPerLine);

            case RewardListArrangeType.WrapByParentSize:
            default:
                Canvas.ForceUpdateCanvases();
                var sr = scrollRect.GetComponent<ScrollRect>();
                var vp = sr.viewport != null ? sr.viewport : (RectTransform)sr.transform;
                float availableWidth = vp.rect.width - linePadding * 2f;
                if (availableWidth <= 0f || iconSize <= 0f) return 1;
                return Mathf.Max(1, Mathf.FloorToInt((availableWidth + iconSpacing) / (iconSize + iconSpacing)));
        }
    }

    // ══════════════════════════════════════════════
    //  Public API
    // ══════════════════════════════════════════════

    /// <summary>正規化されたスクロール位置（0〜1）。</summary>
    public float ScrollPosition
    {
        get => scrollRect.NormalizedPosition;
        set => scrollRect.NormalizedPosition = value;
    }

    /// <summary>
    /// 報酬リストをセットしてスクロールを初期化する。
    /// </summary>
    public void Setup(List<RewardEntry> rewards,
                      RewardDisplayMode mode = default,
                      RewardMergeMode merge = default,
                      RewardListOneLineArrangeType oneLineArrangeType = default,
                      Action onComplete = null,
                      bool canUse = false)
    {
        StopAnimation();

        brockOverlay.enabled = true;
        brockOverlay.transform.SetAsLastSibling();
        if (oneLineArrangeType != default) this.oneLineArrangeType = oneLineArrangeType;
        this.canUse = canUse;
        this.onComplete = onComplete;

        SetSkipVisible(false);
        SetMergeVisible(false);

        // モード設定（default = Inspector の値を使用）
        var activeMode  = mode  != default ? mode  : displayMode;
        var activeMerge = merge != default ? merge : mergeMode;

        rawRewards = new List<RewardEntry>(rewards);

        // マージ適用
        if (activeMerge == RewardMergeMode.PreMerge)
            displayRewards = MergeRewards(rawRewards);
        else
            displayRewards = new List<RewardEntry>(rawRewards);

        // displayRewards 確定後に解決（DistributeByRowCount 等が参照するため）
        resolvedCountPerLine = ResolveCountPerLine();
        lineCount = CalcLineCount(displayRewards.Count);

        // セル高さ・パディング設定（セルサイズにラインスペーシングを含める）
        scrollRect.SetCellMainSize(iconSize + lineSpacing);
        scrollRect.SetPadding(heightPadding, heightPadding);

        if (activeMode == RewardDisplayMode.AllAtOnce)
        {
            revealedCount = displayRewards.Count;
            scrollRect.Init(lineCount, OnInitCell, OnUpdateCell);
            AdjustHeight();
            scrollRect.ScrollToTop(false);
            brockOverlay.enabled = false;
            UpdateMergeVisible();
            this.onComplete?.Invoke();
            this.onComplete = null;
        }
        else // OneByOne
        {
            revealedCount = 0;
            scrollRect.Init(lineCount, OnInitCell, OnUpdateCell);
            AdjustHeight();
            scrollRect.ScrollToTop(false);
            scrollRect.SetScrollable(false);
            SetSkipVisible(true);
            animCoroutine = StartCoroutine(AnimateOneByOne());
        }
    }

    int CalcLineCount(int itemCount)
    {
        return itemCount > 0 ? Mathf.CeilToInt((float)itemCount / resolvedCountPerLine) : 0;
    }

    // ══════════════════════════════════════════════
    //  内部実装
    // ══════════════════════════════════════════════

    private bool needsScroll;

    void AdjustHeight()
    {
        Canvas.ForceUpdateCanvases();

        if (!adjustHeight)
        {
            float viewHeight = rectTransform.rect.height;
            float contentHeight = scrollRect.ResolvedContentLength;
            needsScroll = contentHeight > viewHeight;
            return;
        }

        var parentRT = rectTransform.parent as RectTransform;
        if (parentRT == null) return;

        float parentHeight = parentRT.rect.height;
        float maxHeight = parentHeight - verticalMargin * 2f;
        float cHeight = scrollRect.ResolvedContentLength;
        float height = Mathf.Min(cHeight, maxHeight);
        needsScroll = cHeight > maxHeight;

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    void OnInitCell(MyScrollCell cell)
    {
        var line = cell.Get<RewardLineUI>();
        line.Setup(iconPrefab, resolvedCountPerLine, iconSpacing);
    }

    void OnUpdateCell(int lineIndex, MyScrollCell cell)
    {
        var line = cell.Get<RewardLineUI>();
        line.EnsureIconCount(resolvedCountPerLine);
        int start = lineIndex * resolvedCountPerLine;
        int total = Mathf.Min(resolvedCountPerLine, displayRewards.Count - start);
        int visible = Mathf.Clamp(revealedCount - start, 0, total);
        line.SetLineInfo(lineIndex, total);
        line.SetIconSize(iconSize, visible);
        line.SetData(displayRewards, start, visible, canUse);
        line.LayoutIcons(oneLineArrangeType, linePadding);
        // 既に表示済みアイコンのスケール/アルファを正常に戻す（リサイクル対策）
        for (int i = 0; i < visible; i++)
            line.GetIcon(i)?.ShowImmediate();
    }

    IEnumerator AnimateOneByOne()
    {
        var wait = new WaitForSeconds(itemInterval);
        int prevLineIdx = -1;

        for (int i = 0; i < displayRewards.Count; i++)
        {
            int lineIdx = i / resolvedCountPerLine;
            int iconIdx = i % resolvedCountPerLine;

            // ラインが変わったらスクロール追従
            if (lineIdx != prevLineIdx)
            {
                prevLineIdx = lineIdx;
                if (scrollRect.FollowIndex(lineIdx))
                    yield return new WaitForSeconds(scrollRect.AnimDuration);
            }

            // セル取得 → アイコンにデータ設定 → アニメーション
            var cell = scrollRect.FindVisibleCell(lineIdx);
            if (cell != null)
            {
                var line = cell.Get<RewardLineUI>();
                line.EnsureIconCount(resolvedCountPerLine);
                int totalInLine = Mathf.Min(resolvedCountPerLine, displayRewards.Count - lineIdx * resolvedCountPerLine);
                line.SetLineInfo(lineIdx, totalInLine);
                line.LayoutIcons(oneLineArrangeType, linePadding);
                line.RevealIcon(iconIdx, displayRewards[i].rewardId, displayRewards[i].amount, revealDuration, revealEase);
            }

            revealedCount = i + 1;
            yield return wait;
        }

        FinishAnimation();
    }

    static List<RewardEntry> MergeRewards(List<RewardEntry> source)
    {
        var dict = new Dictionary<int, int>(); // rewardId → total amount
        foreach (var e in source)
        {
            if (dict.ContainsKey(e.rewardId)) dict[e.rewardId] += e.amount;
            else dict[e.rewardId] = e.amount;
        }
        var result = new List<RewardEntry>();
        foreach (var kv in dict)
            result.Add(new RewardEntry { rewardId = kv.Key, amount = kv.Value });
        return result;
    }

    /// <summary>演出をスキップして全アイテムを即座に表示する。</summary>
    public void Skip()
    {
        if (animCoroutine == null) return;
        StopAnimation();
        revealedCount = displayRewards.Count;
        scrollRect.Init(lineCount, OnInitCell, OnUpdateCell);
        AdjustHeight();
        scrollRect.ScrollToTop(false);
        FinishAnimation();
    }

    /// <summary>表示中のリワードをマージして再表示する。</summary>
    public void Merge()
    {
        displayRewards = MergeRewards(displayRewards);
        lineCount = CalcLineCount(displayRewards.Count);
        revealedCount = displayRewards.Count;
        scrollRect.Init(lineCount, OnInitCell, OnUpdateCell);
        AdjustHeight();
        scrollRect.ScrollToTop(false);
        SetMergeVisible(false);
        onComplete?.Invoke();
        onComplete = null;
    }

    /// <summary>演出完了時の共通処理。マージ可能なら onComplete を保留する。</summary>
    void FinishAnimation()
    {
        brockOverlay.enabled = false;
        animCoroutine = null;
        SetSkipVisible(false);
        scrollRect.SetScrollable(needsScroll);
        scrollRect.ScrollToEnd(false);

        if (CanMerge() && mergeButton != null)
        {
            SetMergeVisible(true);
        }
        else
        {
            SetMergeVisible(false);
            onComplete?.Invoke();
            onComplete = null;
        }
    }

    void StopAnimation()
    {
        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
            animCoroutine = null;
        }
    }

    // ── ボタン表示制御 ───────────────────────

    bool CanMerge()
    {
        if (displayRewards == null || displayRewards.Count <= 1) return false;
        var seen = new HashSet<int>();
        foreach (var e in displayRewards)
        {
            if (!seen.Add(e.rewardId)) return true;
        }
        return false;
    }

    void SetSkipVisible(bool visible)
    {
        if (skipButton != null) skipButton.gameObject.SetActiveIfChanged(visible);
    }

    void SetMergeVisible(bool visible)
    {
        if (mergeButton != null) mergeButton.gameObject.SetActiveIfChanged(visible);
    }

    void UpdateMergeVisible() => SetMergeVisible(CanMerge());

#if UNITY_EDITOR
    [FoldoutGroup("デバッグ")]
    [SerializeField] private int debugCount = 100;

    [FoldoutGroup("デバッグ")]
    [Button("テスト生成 (AllAtOnce)")]
    void DebugGenerateAll()
    {
        var list = new List<RewardEntry>();
        for (int i = 0; i < debugCount; i++)
            list.Add(new RewardEntry { rewardId = i + 1, amount = 1 });
        Setup(list, RewardDisplayMode.AllAtOnce, RewardMergeMode.NoMerge);
    }

    [FoldoutGroup("デバッグ")]
    [Button("テスト生成 (OneByOne)")]
    void DebugGenerateOneByOne()
    {
        var list = new List<RewardEntry>();
        for (int i = 0; i < debugCount; i++)
            list.Add(new RewardEntry { rewardId = i + 1, amount = 1 });
        Setup(list, RewardDisplayMode.OneByOne, RewardMergeMode.NoMerge);
    }

    [FoldoutGroup("デバッグ")]
    [Button("テスト生成 (PreMerge)")]
    void DebugGeneratePreMerge()
    {
        var list = new List<RewardEntry>();
        for (int i = 0; i < debugCount; i++)
            list.Add(new RewardEntry { rewardId = (i % 10) + 1, amount = 1 });
        Setup(list, RewardDisplayMode.AllAtOnce, RewardMergeMode.PreMerge);
    }

    [FoldoutGroup("デバッグ")]
    [Button("テスト生成 (PostMerge)")]
    void DebugGeneratePostMerge()
    {
        var list = new List<RewardEntry>();
        for (int i = 0; i < debugCount; i++)
            list.Add(new RewardEntry { rewardId = (i % 10) + 1, amount = 1 });
        Setup(list, RewardDisplayMode.OneByOne, RewardMergeMode.PostMerge);
    }
#endif
}
