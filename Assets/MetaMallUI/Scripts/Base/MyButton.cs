using System;
using System.Collections;
using PrimeTween;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// タップTween・ロングタップ・ロック状態に対応した汎用ボタン。
/// IPointerHandler + コルーチンで実装し、Update を使わない。
/// </summary>
public class MyButton : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    // ── 設定 ──────────────────────────────────
    [Header("Tween")]
    [SerializeField] bool useTween = true;
    [SerializeField] float tweenScale = 0.9f;
    [SerializeField] float tweenDuration = 0.1f;
    [SerializeField] Ease tweenEase = Ease.OutQuad;

    [Header("Long Press")]
    [SerializeField] bool useLongPress;
    [SerializeField] float longPressDuration = 0.6f;

    [Header("Repeat")]
    [SerializeField] bool useRepeat;
    [SerializeField] bool useRepeatAcceleration;
    [SerializeField] float repeatDelay = 0.4f;
    [SerializeField] float repeatIntervalMax = 0.15f;
    [SerializeField] float repeatIntervalMin = 0.03f;
    [SerializeField] float repeatAccelDuration = 2f;

    [Header("State")]
    [SerializeField] bool isLocked;

    // ── コールバック ──────────────────────────
    [Header("Events")]
    [SerializeField] UnityEvent onClick;
    [SerializeField] UnityEvent onLongPress;
    [SerializeField] UnityEvent onLockedClick;

    // ── 内部 ──────────────────────────────────
    Coroutine longPressCoroutine;
    Coroutine repeatCoroutine;
    bool longPressFired;
    Tween currentTween;

    // ── プロパティ ────────────────────────────
    public bool IsLocked
    {
        get => isLocked;
        set => isLocked = value;
    }

    // ── リスナー登録 ────────────────────────
    public void SetOnClick(UnityAction action)
    {
        onClick.RemoveAllListeners();
        onClick.AddListener(action);
    }

    public void SetOnLongPress(UnityAction action)
    {
        onLongPress.RemoveAllListeners();
        onLongPress.AddListener(action);
    }

    public void SetOnLockedClick(UnityAction action)
    {
        onLockedClick.RemoveAllListeners();
        onLockedClick.AddListener(action);
    }

    // ── Pointer Events ───────────────────────
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isLocked) return;

        // 押下 Tween
        if (useTween)
        {
            currentTween.Stop();
            currentTween = Tween.Scale(transform, Vector3.one * tweenScale, tweenDuration, tweenEase);
        }

        // ロングタップ計測開始
        if (useLongPress)
        {
            longPressFired = false;
            longPressCoroutine = StartCoroutine(LongPressRoutine());
        }

        // リピート開始
        if (useRepeat)
        {
            CancelRepeat();
            repeatCoroutine = StartCoroutine(RepeatRoutine());
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CancelLongPress();
        CancelRepeat();

        if (isLocked) return;

        // 戻り Tween
        if (useTween)
        {
            currentTween.Stop();
            currentTween = Tween.Scale(transform, Vector3.one, tweenDuration, tweenEase);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isLocked)
        {
            onLockedClick?.Invoke();
            return;
        }

        // ロングタップが発火済みなら通常クリックは無視
        if (longPressFired) return;

        // リピートモードではPointerDown側で発火済み
        if (useRepeat) return;

        onClick?.Invoke();
    }

    // ── ロングタップ ─────────────────────────
    IEnumerator LongPressRoutine()
    {
        yield return new WaitForSeconds(longPressDuration);
        longPressFired = true;
        onLongPress?.Invoke();
    }

    void CancelLongPress()
    {
        if (longPressCoroutine != null)
        {
            StopCoroutine(longPressCoroutine);
            longPressCoroutine = null;
        }
    }

    // ── リピート ───────────────────────────────
    IEnumerator RepeatRoutine()
    {
        onClick?.Invoke();
        yield return new WaitForSeconds(repeatDelay);

        float elapsed = 0f;
        while (true)
        {
            float interval = useRepeatAcceleration
                ? Mathf.Lerp(repeatIntervalMax, repeatIntervalMin,
                    Mathf.Clamp01(elapsed / repeatAccelDuration))
                : repeatIntervalMax;

            yield return new WaitForSeconds(interval);
            elapsed += interval;
            onClick?.Invoke();
        }
    }

    void CancelRepeat()
    {
        if (repeatCoroutine != null)
        {
            StopCoroutine(repeatCoroutine);
            repeatCoroutine = null;
        }
    }

    // ── ライフサイクル ────────────────────────
    void OnDisable()
    {
        CancelLongPress();
        CancelRepeat();
        currentTween.Stop();
        transform.localScale = Vector3.one;
    }
}
