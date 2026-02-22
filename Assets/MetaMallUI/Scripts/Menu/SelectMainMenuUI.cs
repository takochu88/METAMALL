using PrimeTween;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class SelectMainMenuUI : MonoBehaviour
{
    public SelectMenuRowUI selectTopMenuRowUIRight;
    public SelectMenuRowUI selectTopMenuRowUILeft;
    public SelectMenuRowUI selectStageMenuRowUI;
    public SelectMenuRowUI selectGameEventMenuRowUI;
    public SelectMenuRowUI selectPurchaseMenuRowUI;
    public SelectMenuRowUI selectGachaMenuRowUI;
    public SelectMenuRowUI selectStoryMenuRowUI;

    [SerializeField] float showDuration = 0.15f;
    [SerializeField] Ease showEase = Ease.OutCubic;

    CanvasGroup _canvasGroup;
    CanvasGroup canvasGroup => _canvasGroup ??= GetComponent<CanvasGroup>();
    Tween tween;

    public void Show()
    {
        tween.Stop();
        tween = Tween.Alpha(canvasGroup, 1f, showDuration, showEase);
    }

    public void HideImmediate()
    {
        tween.Stop();
        canvasGroup.alpha = 0f;
    }

    void OnDisable()
    {
        tween.Stop();
    }
}
