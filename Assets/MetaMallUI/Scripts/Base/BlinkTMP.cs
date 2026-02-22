using PrimeTween;
using TMPro;
using UnityEngine;

/// <summary>
/// TMP_Text にアタッチするだけで明滅する。
/// CanvasGroup の Alpha を PrimeTween でループ。Update 不使用。
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class BlinkTMP : MonoBehaviour
{
    [SerializeField] float duration = 1f;
    [SerializeField, Range(0f, 1f)] float minAlpha = 0.3f;
    [SerializeField] Ease ease = Ease.InOutSine;

    CanvasGroup _cg;
    Tween tween;

    void Awake()
    {
        _cg = GetComponent<CanvasGroup>();
        if (_cg == null) _cg = gameObject.AddComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        _cg.alpha = 1f;
        tween = Tween.Alpha(_cg, 1f, minAlpha, duration, ease, -1, CycleMode.Yoyo);
    }

    void OnDisable()
    {
        tween.Stop();
    }
}
