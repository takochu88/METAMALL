using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
public class OverlayUIBase : MonoBehaviour
{
    // ── 設定 ─────────────────────────────────
    [FoldoutGroup("Animation")]
    [SerializeField] float showDuration = 0.25f;
    [FoldoutGroup("Animation")]
    [SerializeField] float hideDuration = 0.2f;
    [FoldoutGroup("Animation")]
    [SerializeField] Ease showEase = Ease.OutCubic;
    [FoldoutGroup("Animation")]
    [SerializeField] Ease hideEase = Ease.InCubic;

    [FoldoutGroup("Blur")]
    [SerializeField] bool useBlur;
    [FoldoutGroup("Blur")]
    [ShowIf("useBlur")]
    [SerializeField] BlurBackground blurBackground;

    Color darkColor = new Color(0f, 0f, 0f, 220f / 255f);

    [FoldoutGroup("Close")]
    [SerializeField] MyButton closeButton;

    // ── 状態 ─────────────────────────────────
    enum State { Hidden = 0, Shown = 1 }

    [SerializeField, ReadOnly] State currentState;
    Tween tween;
    bool isFragile;
    OverlayStack parentStack;

    // ── キャッシュ ───────────────────────────
    Canvas _canvas;
    CanvasGroup _canvasGroup;

    Canvas canvas => _canvas ??= GetComponent<Canvas>();
    CanvasGroup canvasGroup => _canvasGroup ??= GetComponent<CanvasGroup>();

    // ── プロパティ ───────────────────────────
    public bool IsShown => currentState == State.Shown;
    public int SortingOrder => canvas.sortingOrder;

    // ── 初期化（OverlayStackから呼ばれる） ──
    public void Setup(OverlayStack parent)
    {
        parentStack = parent;
        CreateDarkImage();
        canvas.enabled = false;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        currentState = State.Hidden;
    }

    void CreateDarkImage()
    {
        var darkGO = new GameObject("Dark");
        darkGO.transform.SetParent(transform, false);
        darkGO.transform.SetAsFirstSibling();

        var rt = darkGO.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        var img = darkGO.AddComponent<UnityEngine.UI.Image>();
        img.color = darkColor;
        img.raycastTarget = true;
    }

    // ── 表示 ─────────────────────────────────
    public void Show()
    {
        if (IsShown) return;
        currentState = State.Shown;
        tween.Stop();

        if (parentStack.IsAnyShown)
            parentStack.Current.OnDeactivated();

        parentStack.Push(this);
        OnBeforeShow();

        if (useBlur && blurBackground != null)
        {
            // キャプチャ完了まで非表示のまま待ち、完了後にShow開始
            blurBackground.Capture(() =>
            {
                canvas.enabled = true;
                canvasGroup.blocksRaycasts = true;
                tween = PlayShow();
                OnAfterShow();
            });
        }
        else
        {
            canvas.enabled = true;
            canvasGroup.blocksRaycasts = true;
            tween = PlayShow();
            OnAfterShow();
        }
    }

    public void ShowImmediate()
    {
        if (IsShown) return;
        currentState = State.Shown;
        tween.Stop();

        if (parentStack.IsAnyShown)
            parentStack.Current.OnDeactivated();

        parentStack.Push(this);
        OnBeforeShow();

        if (useBlur && blurBackground != null)
        {
            blurBackground.Capture(() =>
            {
                canvas.enabled = true;
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
                OnAfterShow();
            });
        }
        else
        {
            canvas.enabled = true;
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            OnAfterShow();
        }
    }

    // ── 非表示 ───────────────────────────────
    public void Hide()
    {
        if (!IsShown) return;
        currentState = State.Hidden;
        tween.Stop();
        canvasGroup.blocksRaycasts = false;

        tween = PlayHide()
            .OnComplete(this, self =>
            {
                self.canvas.enabled = false;

                if (self.useBlur && self.blurBackground != null)
                    self.blurBackground.Release();

                self.parentStack.Remove(self);
                self.OnAfterHide();

                if (self.isFragile)
                    Destroy(self.gameObject);
            });
    }

    public void HideImmediate()
    {
        if (!IsShown) return;
        currentState = State.Hidden;
        tween.Stop();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0f;
        canvas.enabled = false;

        if (useBlur && blurBackground != null)
            blurBackground.Release();

        parentStack.Remove(this);
        OnAfterHide();

        if (isFragile)
            Destroy(gameObject);
    }

    // ── 内部設定 ─────────────────────────────
    internal void SetFragile(bool fragile) => isFragile = fragile;
    internal void SetSortingOrder(int order) => canvas.sortingOrder = order;

    // ── アニメーション（virtual） ────────────
    protected virtual Tween PlayShow()
    {
        return Tween.Alpha(canvasGroup, 1f, showDuration, showEase);
    }

    protected virtual Tween PlayHide()
    {
        return Tween.Alpha(canvasGroup, 0f, hideDuration, hideEase);
    }

    // ── ライフサイクルコールバック（virtual） ─
    protected virtual void OnBeforeShow() { }
    protected virtual void OnAfterShow() { }
    public virtual void OnActivated() { }
    public virtual void OnDeactivated() { }
    protected virtual void OnAfterHide() { }

    void OnDisable()
    {
        tween.Stop();
    }

#if UNITY_EDITOR
    protected virtual void Reset()
    {
        var found = GetComponentsInChildren<MyButton>(true);
        foreach (var btn in found)
        {
            if (btn.gameObject.name == "CloseButton")
            {
                closeButton = btn;
                SetPersistentOnClick(btn);
                break;
            }
        }
    }

    void SetPersistentOnClick(MyButton btn)
    {
        var field = typeof(MyButton).GetField("onClick",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field == null) return;

        var unityEvent = (UnityEngine.Events.UnityEvent)field.GetValue(btn);

        // 既に登録済みなら何もしない
        for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
        {
            if (unityEvent.GetPersistentTarget(i) == (Object)this &&
                unityEvent.GetPersistentMethodName(i) == nameof(Hide))
                return;
        }

        UnityEditor.Events.UnityEventTools.AddPersistentListener(unityEvent, Hide);
        UnityEditor.EditorUtility.SetDirty(btn);
    }
#endif
}
