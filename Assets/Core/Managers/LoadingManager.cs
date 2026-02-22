using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform loadingRect;
    [SerializeField] private Image blocker;
    [SerializeField] private TMP_Text messageLabel;

    [SerializeField] private float fadeDuration = 0.15f;
    [SerializeField] private float timeout = 30f;

    private Coroutine current;

    public void Show(LoadingType type = LoadingType.Loading, float duration = -1)
    {
        ApplyType(type);
        Show(Vector2.zero, duration);
    }

    public void Show(Vector2 position, float duration = -1)
    {
        if (current != null) StopCoroutine(current);

        loadingRect.anchoredPosition = position;
        current = StartCoroutine(ShowRoutine(duration));
    }

    private void ApplyType(LoadingType type)
    {
        if (messageLabel == null) return;
        messageLabel.text = type switch
        {
            LoadingType.Communication => "通信中...",
            LoadingType.Boot          => "起動中...",
            _                         => "ロード中...",
        };
    }

    public void Hide()
    {
        if (current != null) StopCoroutine(current);
        current = StartCoroutine(HideRoutine());
    }

    IEnumerator ShowRoutine(float duration)
    {
        canvas.enabled = true;
        canvasGroup.alpha = 0f;
        blocker.enabled = true;

        // フェードイン
        for (float t = 0f; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            canvasGroup.alpha = t / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // duration指定あり → 自動Hide / なし → timeout秒で安全消去
        float wait = duration >= 0 ? duration : timeout;
        yield return new WaitForSecondsRealtime(wait);

        yield return HideRoutine();
    }

    IEnumerator HideRoutine()
    {
        // フェードアウト
        for (float t = 0f; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            canvasGroup.alpha = 1f - t / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 0f;

        blocker.enabled = false;
        canvas.enabled = false;
        current = null;
    }
}
