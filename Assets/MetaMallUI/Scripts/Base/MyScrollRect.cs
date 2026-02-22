using System;
using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// セル再利用型の無限スクロール。Update 不使用。
/// ScrollRect.onValueChanged + PrimeTween で駆動。
/// </summary>
[RequireComponent(typeof(ScrollRect))]
public class MyScrollRect : MonoBehaviour
{
    // ── 列挙 ──────────────────────────────────
    public enum ScrollDirection { Vertical, Horizontal }
    public enum CellSizeMode { Prefab, Manual, Stretch }

    // ── 設定 ──────────────────────────────────
    [BoxGroup("基本設定")]
    [SerializeField] ScrollDirection direction = ScrollDirection.Vertical;
    [BoxGroup("基本設定")]
    [SerializeField] RectTransform cellPrefab;
    [BoxGroup("基本設定")]
    [SerializeField] float spacing = 0f;

    [BoxGroup("セルサイズ")]
    [LabelText("横 (X)")]
    [SerializeField] CellSizeMode cellSizeModeX = CellSizeMode.Stretch;
    [BoxGroup("セルサイズ")]
    [ShowIf("cellSizeModeX", CellSizeMode.Manual)]
    [SerializeField] float cellWidth = 100f;
    [BoxGroup("セルサイズ")]
    [LabelText("縦 (Y)")]
    [SerializeField] CellSizeMode cellSizeModeY = CellSizeMode.Prefab;
    [BoxGroup("セルサイズ")]
    [ShowIf("cellSizeModeY", CellSizeMode.Manual)]
    [SerializeField] float cellHeight = 100f;

    [BoxGroup("パディング")]
    [SerializeField] float paddingTop;
    [BoxGroup("パディング")]
    [SerializeField] float paddingBottom;
    [BoxGroup("パディング")]
    [SerializeField] float paddingLeft;
    [BoxGroup("パディング")]
    [SerializeField] float paddingRight;

    [BoxGroup("スクロール制御")]
    [Tooltip("コンテンツがビューポートに収まる場合、スクロールを無効にする")]
    [SerializeField] private bool disableScrollWhenNotNeeded = true;

    [BoxGroup("スクロール制御")]
    [Tooltip("Scrollbar.size を Inspector の初期値で固定する（ScrollRect の自動サイズ変更を無効にする）")]
    [SerializeField] private bool preserveScrollbarSize;

    [BoxGroup("アニメーション")]
    [SerializeField] float animDuration = 0.3f;
    [BoxGroup("アニメーション")]
    [SerializeField] Ease animEase = Ease.OutCubic;

    // ── 内部 ──────────────────────────────────
    ScrollRect _scrollRect;
    RectTransform content;
    RectTransform viewport;
    Transform poolRoot;
    bool isInitialized;

    ScrollRect scrollRect
    {
        get
        {
            EnsureInitialized();
            return _scrollRect;
        }
    }

    readonly List<MyScrollCell> visibleCells = new();
    readonly Stack<MyScrollCell> pool = new();

    int totalCount;
    int currentFirst = -1;
    int currentLast  = -1;

    Action<MyScrollCell> onInitCell;
    Action<int, MyScrollCell> onUpdateCell;
    Tween scrollTween;

    // 解決済みセルサイズ（Init 時に確定）
    private float resolvedWidth;
    private float resolvedHeight;
    private float scrollbarThickness;
    private float scrollbarSpacing;

    // Scrollbar サイズ保持用
    private Scrollbar managedScrollbar;
    private float initialScrollbarSize;

    // ── ヘルパー ──────────────────────────────
    bool  IsVertical   => direction == ScrollDirection.Vertical;
    float CellMain     => IsVertical ? resolvedHeight : resolvedWidth;
    float Stride       => CellMain + spacing;
    float PaddingHead  => IsVertical ? paddingTop : paddingLeft;
    float PaddingTail  => IsVertical ? paddingBottom : paddingRight;
    float ViewportSize => IsVertical ? viewport.rect.height : viewport.rect.width;
    float ContentLength => totalCount > 0
        ? PaddingHead + totalCount * CellMain + (totalCount - 1) * spacing + PaddingTail
        : 0f;
    float MaxScroll => Mathf.Max(0f, ContentLength - ViewportSize);

    // ── ライフサイクル ────────────────────────
    void EnsureInitialized()
    {
        if (isInitialized) return;
        isInitialized = true;

        _scrollRect = GetComponent<ScrollRect>();
        content     = _scrollRect.content;
        viewport    = _scrollRect.viewport != null
            ? _scrollRect.viewport
            : (RectTransform)_scrollRect.transform;

        // AutoHideAndExpandViewport が Viewport / Scrollbar の
        // RectTransform を壊すため Permanent に切り替えて修復する
        FixScrollbarLayout();

        // Scrollbar サイズ保持モード: ScrollRect から切り離して手動管理
        if (preserveScrollbarSize)
        {
            managedScrollbar = IsVertical ? _scrollRect.verticalScrollbar : _scrollRect.horizontalScrollbar;
            if (managedScrollbar != null)
            {
                initialScrollbarSize = managedScrollbar.size;
                if (IsVertical) _scrollRect.verticalScrollbar = null;
                else            _scrollRect.horizontalScrollbar = null;
                managedScrollbar.onValueChanged.AddListener(OnScrollbarDragged);
            }
        }

        // プール用コンテナ（content の外に配置してヒエラルキーを整理）
        var poolGo = new GameObject("_Pool", typeof(RectTransform));
        poolGo.transform.SetParent(transform, false);
        poolRoot = poolGo.transform;

        _scrollRect.onValueChanged.AddListener(_ => RefreshVisibleCells());
    }

    void FixScrollbarLayout()
    {
        var sb = IsVertical ? _scrollRect.verticalScrollbar : _scrollRect.horizontalScrollbar;
        if (sb == null) return;

        // Visibility を Permanent にして Viewport の自動リサイズを止める
        if (IsVertical)
            _scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
        else
            _scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;

        var sbRT = (RectTransform)sb.transform;
        scrollbarThickness = IsVertical ? sbRT.sizeDelta.x : sbRT.sizeDelta.y;
        scrollbarSpacing = IsVertical
            ? _scrollRect.verticalScrollbarSpacing
            : _scrollRect.horizontalScrollbarSpacing;

        // Scrollbar: スクロール方向に沿ってストレッチ
        if (IsVertical)
        {
            sbRT.anchorMin = new Vector2(1f, 0f);
            sbRT.anchorMax = new Vector2(1f, 1f);
            sbRT.sizeDelta = new Vector2(scrollbarThickness, 0f);
        }
        else
        {
            sbRT.anchorMin = new Vector2(0f, 0f);
            sbRT.anchorMax = new Vector2(1f, 0f);
            sbRT.sizeDelta = new Vector2(0f, scrollbarThickness);
        }

        // Viewport: Scrollbar 分を引いてストレッチ
        viewport.anchorMin = Vector2.zero;
        viewport.anchorMax = Vector2.one;
        viewport.anchoredPosition = Vector2.zero;
        if (IsVertical)
            viewport.sizeDelta = new Vector2(-(scrollbarThickness + scrollbarSpacing), 0f);
        else
            viewport.sizeDelta = new Vector2(0f, -(scrollbarThickness + scrollbarSpacing));
    }

    void OnDisable()
    {
        scrollTween.Stop();
    }

    // ══════════════════════════════════════════
    //  Public API
    // ══════════════════════════════════════════

    /// <summary>
    /// スクロールを初期化する（onInit なし）。
    /// </summary>
    public void Init(int count, Action<int, MyScrollCell> onUpdate)
    {
        Init(count, null, onUpdate);
    }

    /// <summary>
    /// スクロールを初期化する。
    /// </summary>
    /// <param name="count">総アイテム数</param>
    /// <param name="onInit">セル新規生成時コールバック (cell)。ボタン登録など1回だけの処理用</param>
    /// <param name="onUpdate">セル更新コールバック (index, cell)。スクロールで再利用されるたびに呼ばれる</param>
    public void Init(int count, Action<MyScrollCell> onInit, Action<int, MyScrollCell> onUpdate)
    {
        scrollTween.Stop();
        scrollRect.velocity = Vector2.zero;

        // セルサイズ解決
        ResolveCellSize();

        totalCount   = count;
        onInitCell   = onInit;
        onUpdateCell = onUpdate;
        currentFirst = currentLast = -1;

        // プール回収
        foreach (var c in visibleCells) ReturnToPool(c);
        visibleCells.Clear();

        // Content 内の管理外オブジェクトをプールへ退避
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            var child = content.GetChild(i);
            if (child.GetComponent<MyScrollCell>() == null)
                child.SetParent(poolRoot, false);
        }

        // Content のアンカーをスクロール方向に合わせて設定
        // ストレッチはスクロール軸と直交する方向のみにする
        if (IsVertical)
        {
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot     = new Vector2(0.5f, 1f);
            content.sizeDelta = new Vector2(0f, ContentLength);
        }
        else
        {
            content.anchorMin = new Vector2(0f, 0f);
            content.anchorMax = new Vector2(0f, 1f);
            content.pivot     = new Vector2(0f, 0.5f);
            content.sizeDelta = new Vector2(ContentLength, 0f);
        }

        // スクロール不要なら無効化
        if (disableScrollWhenNotNeeded)
        {
            bool need = ContentLength > ViewportSize;
            if (IsVertical) scrollRect.vertical   = need;
            else            scrollRect.horizontal = need;
        }

        content.anchoredPosition = Vector2.zero;
        RefreshVisibleCells();
    }

    /// <summary>先頭へスクロール。</summary>
    public void ScrollToTop(bool animated = true)
    {
        ScrollTo(Vector2.zero, animated);
    }

    /// <summary>末尾へスクロール。</summary>
    public void ScrollToEnd(bool animated = true)
    {
        var target = IsVertical
            ? new Vector2(0f, MaxScroll)
            : new Vector2(-MaxScroll, 0f);
        ScrollTo(target, animated);
    }

    /// <summary>
    /// 指定 Index の位置へスクロール。
    /// </summary>
    /// <param name="index">移動先のセルインデックス</param>
    /// <param name="alignment">
    /// 0 = セルをビューポートの上端(左端)に配置
    /// 0.5 = セルをビューポートの中央に配置
    /// 1 = セルをビューポートの下端(右端)に配置
    /// </param>
    /// <param name="animated">アニメーションの有無</param>
    public void ScrollToIndex(int index, float alignment = 0f, bool animated = true)
    {
        index = Mathf.Clamp(index, 0, Mathf.Max(0, totalCount - 1));
        alignment = Mathf.Clamp01(alignment);
        float cellStart = PaddingHead + index * Stride;
        float pos = Mathf.Clamp(cellStart - alignment * (ViewportSize - CellMain), 0f, MaxScroll);
        var target = IsVertical
            ? new Vector2(0f, pos)
            : new Vector2(-pos, 0f);
        ScrollTo(target, animated);
    }

    /// <summary>
    /// 指定インデックスがビューポート内に完全に収まるようスクロールする。
    /// 既に収まっている場合は何もせず false を返す。
    /// </summary>
    public bool FollowIndex(int index, float alignment = 1f)
    {
        EnsureInitialized();
        index = Mathf.Clamp(index, 0, Mathf.Max(0, totalCount - 1));

        float scroll = IsVertical
            ? content.anchoredPosition.y
            : -content.anchoredPosition.x;
        scroll = Mathf.Max(0f, scroll);

        float cellStart = PaddingHead + index * Stride;
        float cellEnd = cellStart + Stride + PaddingTail;
        float viewEnd = scroll + ViewportSize;

        if (cellEnd <= viewEnd) return false;

        // cellEnd がビューポート下端に来る位置へスクロール
        float pos = Mathf.Clamp(cellEnd - ViewportSize, 0f, MaxScroll);
        var target = IsVertical
            ? new Vector2(0f, pos)
            : new Vector2(-pos, 0f);
        ScrollTo(target, true);
        return true;
    }

    /// <summary>セルの高さ(縦スクロール時)または幅(横スクロール時)をコードから設定する。次回 Init で反映。</summary>
    public void SetCellMainSize(float size)
    {
        if (IsVertical)
        {
            cellSizeModeY = CellSizeMode.Manual;
            cellHeight = size;
        }
        else
        {
            cellSizeModeX = CellSizeMode.Manual;
            cellWidth = size;
        }
    }

    /// <summary>パディングをコードから設定。</summary>
    public void SetPadding(float top, float bottom)
    {
        paddingTop = top;
        paddingBottom = bottom;
    }

    /// <summary>アイテム総数を動的に更新（ライン追加用）。</summary>
    public void UpdateCount(int newCount)
    {
        totalCount = newCount;
        if (IsVertical) content.sizeDelta = new Vector2(content.sizeDelta.x, ContentLength);
        else            content.sizeDelta = new Vector2(ContentLength, content.sizeDelta.y);
        if (disableScrollWhenNotNeeded)
        {
            bool need = ContentLength > ViewportSize;
            if (IsVertical) scrollRect.vertical = need;
            else            scrollRect.horizontal = need;
        }
        RefreshVisibleCells();
    }

    /// <summary>指定インデックスの可視セルを検索。</summary>
    public MyScrollCell FindVisibleCell(int index)
    {
        foreach (var cell in visibleCells)
            if (cell.Index == index) return cell;
        return null;
    }

    /// <summary>アニメーション時間の公開。</summary>
    public float AnimDuration => animDuration;

    /// <summary>総アイテム数。</summary>
    public int TotalCount => totalCount;

    /// <summary>1セルあたりの高さ(幅) + spacing。Init後に有効。</summary>
    public float ResolvedStride => Stride;

    /// <summary>パディング含む全コンテンツ長。Init後に有効。</summary>
    public float ResolvedContentLength => ContentLength;

    /// <summary>正規化されたスクロール位置（0〜1）。</summary>
    public float NormalizedPosition
    {
        get
        {
            EnsureInitialized();
            return IsVertical
                ? _scrollRect.verticalNormalizedPosition
                : _scrollRect.horizontalNormalizedPosition;
        }
        set
        {
            EnsureInitialized();
            scrollTween.Stop();
            _scrollRect.velocity = Vector2.zero;
            if (IsVertical) _scrollRect.verticalNormalizedPosition = value;
            else            _scrollRect.horizontalNormalizedPosition = value;
            RefreshVisibleCells();
        }
    }

    /// <summary>スクロール操作の有効/無効を切り替える。無効時はスクロールバーを隠してビューポートを広げる。</summary>
    public void SetScrollable(bool scrollable)
    {
        if (IsVertical) scrollRect.vertical   = scrollable;
        else            scrollRect.horizontal = scrollable;

        var sb = managedScrollbar != null
            ? managedScrollbar
            : (IsVertical ? scrollRect.verticalScrollbar : scrollRect.horizontalScrollbar);
        if (sb != null)
        {
            sb.gameObject.SetActiveIfChanged(scrollable);
            float offset = scrollable ? -(scrollbarThickness + scrollbarSpacing) : 0f;
            if (IsVertical)
                viewport.sizeDelta = new Vector2(offset, 0f);
            else
                viewport.sizeDelta = new Vector2(0f, offset);
        }
    }

    // ══════════════════════════════════════════
    //  デバッグ（実行中のみ）
    // ══════════════════════════════════════════

#if UNITY_EDITOR
    [BoxGroup("デバッグ")]
    [SerializeField, PropertyOrder(100)]
    int debugTargetIndex;

    [BoxGroup("デバッグ")]
    [SerializeField, PropertyOrder(100), Range(0f, 1f)]
    [Tooltip("0=上(左)  0.5=中央  1=下(右)")]
    float debugAlignment = 0.5f;

    [BoxGroup("デバッグ"), ButtonGroup("デバッグ/nav"), PropertyOrder(101)]
    [Button("先頭へ")]
    void DebugScrollToTop() => ScrollToTop();

    [BoxGroup("デバッグ"), ButtonGroup("デバッグ/nav"), PropertyOrder(101)]
    [Button("末尾へ")]
    void DebugScrollToEnd() => ScrollToEnd();

    [BoxGroup("デバッグ"), ButtonGroup("デバッグ/idx"), PropertyOrder(102)]
    [Button("Indexへ (アニメ)")]
    void DebugScrollToIndex() => ScrollToIndex(debugTargetIndex, debugAlignment);

    [BoxGroup("デバッグ"), ButtonGroup("デバッグ/idx"), PropertyOrder(102)]
    [Button("Indexへ (即座)")]
    void DebugScrollToIndexImmediate() => ScrollToIndex(debugTargetIndex, debugAlignment, false);

    [BoxGroup("デバッグ"), ButtonGroup("デバッグ/reset"), PropertyOrder(103)]
    [Button("リセット (アニメ)")]
    void DebugResetAnimated() => ScrollToTop();

    [BoxGroup("デバッグ"), ButtonGroup("デバッグ/reset"), PropertyOrder(103)]
    [Button("リセット (即座)")]
    void DebugResetImmediate() => ScrollToTop(false);
#endif

    // ══════════════════════════════════════════
    //  内部実装
    // ══════════════════════════════════════════

    void ResolveCellSize()
    {
        resolvedWidth  = ResolveAxis(cellSizeModeX, cellWidth, true);
        resolvedHeight = ResolveAxis(cellSizeModeY, cellHeight, false);
    }

    float ResolveAxis(CellSizeMode mode, float manualSize, bool isX)
    {
        switch (mode)
        {
            case CellSizeMode.Stretch:
                return isX
                    ? viewport.rect.width - paddingLeft - paddingRight
                    : viewport.rect.height - paddingTop - paddingBottom;
            case CellSizeMode.Manual:
                return manualSize;
            default: // Prefab
                if (cellPrefab == null) return manualSize;
                if (isX)
                {
                    bool stretch = !Mathf.Approximately(cellPrefab.anchorMin.x, cellPrefab.anchorMax.x);
                    return stretch ? manualSize : cellPrefab.rect.width;
                }
                else
                {
                    bool stretch = !Mathf.Approximately(cellPrefab.anchorMin.y, cellPrefab.anchorMax.y);
                    return stretch ? manualSize : cellPrefab.rect.height;
                }
        }
    }

    void ScrollTo(Vector2 target, bool animated)
    {
        scrollTween.Stop();
        scrollRect.velocity = Vector2.zero;

        if (!animated)
        {
            content.anchoredPosition = target;
            RefreshVisibleCells();
            return;
        }

        var from = content.anchoredPosition;
        scrollTween = Tween.Custom(this, 0f, 1f, animDuration, (self, t) =>
        {
            self.content.anchoredPosition = Vector2.LerpUnclamped(from, target, t);
            self.RefreshVisibleCells();
        }, animEase);
    }

    void RefreshVisibleCells()
    {
        SyncScrollbar();

        if (totalCount <= 0 || Stride <= 0f) return;

        float scroll = IsVertical
            ? content.anchoredPosition.y
            : -content.anchoredPosition.x;
        scroll = Mathf.Max(0f, scroll);

        // パディング分を引いてからインデックスを計算
        float scrollInContent = scroll - PaddingHead;
        int newFirst = Mathf.Max(0, Mathf.FloorToInt(scrollInContent / Stride) - 1);
        int visible  = Mathf.CeilToInt(ViewportSize / Stride) + 3;
        int newLast  = Mathf.Min(totalCount - 1, newFirst + visible);

        if (newFirst == currentFirst && newLast == currentLast) return;

        // 範囲外セルを回収
        for (int i = visibleCells.Count - 1; i >= 0; i--)
        {
            var c = visibleCells[i];
            if (c.Index < newFirst || c.Index > newLast)
            {
                ReturnToPool(c);
                visibleCells.RemoveAt(i);
            }
        }

        // 新規セルを配置
        for (int idx = newFirst; idx <= newLast; idx++)
        {
            if (HasCell(idx)) continue;
            var c = GetFromPool();
            c.Index = idx;
            PositionCell(c);
            onUpdateCell?.Invoke(idx, c);
            visibleCells.Add(c);
        }

        // ヒエラルキー順を index 順に揃える（描画順に影響）
        visibleCells.Sort((a, b) => a.Index.CompareTo(b.Index));
        for (int i = 0; i < visibleCells.Count; i++)
            visibleCells[i].transform.SetSiblingIndex(i);

        currentFirst = newFirst;
        currentLast  = newLast;
    }

    bool HasCell(int index)
    {
        foreach (var c in visibleCells)
            if (c.Index == index) return true;
        return false;
    }

    void PositionCell(MyScrollCell cell)
    {
        float mainPos = PaddingHead + cell.Index * Stride;

        if (IsVertical)
        {
            float x = cellSizeModeX == CellSizeMode.Stretch ? 0f : paddingLeft;
            cell.RectTransform.anchoredPosition = new Vector2(x, -mainPos);
        }
        else
        {
            float y = cellSizeModeY == CellSizeMode.Stretch ? 0f : -paddingTop;
            cell.RectTransform.anchoredPosition = new Vector2(mainPos, y);
        }
    }

    MyScrollCell GetFromPool()
    {
        MyScrollCell cell;

        if (pool.Count > 0)
        {
            cell = pool.Pop();
            cell.transform.SetParent(content, false);
            cell.gameObject.SetActive(true);
        }
        else
        {
            var go = Instantiate(cellPrefab, content);
            cell = go.GetComponent<MyScrollCell>();
            if (cell == null) cell = go.gameObject.AddComponent<MyScrollCell>();
            onInitCell?.Invoke(cell);
        }

        ApplyCellSize(cell.RectTransform);
        return cell;
    }

    void ApplyCellSize(RectTransform rt)
    {
        bool stretchX = cellSizeModeX == CellSizeMode.Stretch;
        bool stretchY = cellSizeModeY == CellSizeMode.Stretch;

        if (IsVertical)
        {
            rt.anchorMin = new Vector2(stretchX ? 0f : 0f, 1f);
            rt.anchorMax = new Vector2(stretchX ? 1f : 0f, 1f);
            rt.pivot     = new Vector2(stretchX ? 0.5f : 0f, 1f);
            rt.sizeDelta = new Vector2(
                stretchX ? -(paddingLeft + paddingRight) : resolvedWidth,
                resolvedHeight);
        }
        else
        {
            rt.anchorMin = new Vector2(0f, stretchY ? 0f : 0f);
            rt.anchorMax = new Vector2(0f, stretchY ? 1f : 0f);
            rt.pivot     = new Vector2(0f, stretchY ? 0.5f : 0f);
            rt.sizeDelta = new Vector2(
                resolvedWidth,
                stretchY ? -(paddingTop + paddingBottom) : resolvedHeight);
        }
    }

    void ReturnToPool(MyScrollCell cell)
    {
        cell.gameObject.SetActive(false);
        cell.transform.SetParent(poolRoot, false);
        pool.Push(cell);
    }

    // ── Scrollbar サイズ保持 ──────────────────

    /// <summary>切り離した Scrollbar の value を content 位置に同期し、size を初期値に固定する。</summary>
    void SyncScrollbar()
    {
        if (managedScrollbar == null) return;
        float normalized = IsVertical
            ? _scrollRect.verticalNormalizedPosition
            : _scrollRect.horizontalNormalizedPosition;
        managedScrollbar.SetValueWithoutNotify(Mathf.Clamp01(normalized));
        managedScrollbar.size = initialScrollbarSize;
    }

    /// <summary>ユーザーが Scrollbar をドラッグしたときに content 位置を追従させる。</summary>
    void OnScrollbarDragged(float value)
    {
        _scrollRect.velocity = Vector2.zero;
        if (IsVertical) _scrollRect.verticalNormalizedPosition = value;
        else            _scrollRect.horizontalNormalizedPosition = value;
        RefreshVisibleCells();
    }
}
