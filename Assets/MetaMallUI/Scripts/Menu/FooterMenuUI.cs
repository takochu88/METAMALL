using PrimeTween;
using TMPro;
using UnityEngine;

public class FooterMenuUI : MonoBehaviour
{
    [SerializeField] TMP_Text menuTitleText;
    [SerializeField] float fadeDuration = 0.2f;
    [SerializeField] Ease fadeEase = Ease.OutCubic;

    Tween tween;

    public void SetMenuTitle(string title)
    {
        tween.Stop();
        menuTitleText.alpha = 0f;
        menuTitleText.text = title;
        tween = Tween.Alpha(menuTitleText, 1f, fadeDuration, fadeEase);
    }

    void OnDisable()
    {
        tween.Stop();
    }
}
