using System.Collections;
using UnityEngine;
using TMPro;

public class ToastManager : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] TMP_Text label;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] RectTransform toastRect;

    [SerializeField] float defaultDuration = 2f;
    [SerializeField] float fadeDuration = 0.25f;

    Coroutine current;

    public void Show(string text, float duration = -1)
    {
        Show(text, Vector2.zero, duration);
    }

    public void Show(string text, Vector2 position, float duration = -1)
    {
        if (current != null) StopCoroutine(current);

        label.text = text;
        toastRect.anchoredPosition = position;
        float dur = duration < 0 ? defaultDuration : duration;
        current = StartCoroutine(ToastRoutine(dur));
    }

    IEnumerator ToastRoutine(float duration)
    {
        canvas.enabled = true;
        canvasGroup.alpha = 0f;

        // フェードイン
        for (float t = 0f; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            canvasGroup.alpha = t / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // 表示待機
        yield return new WaitForSecondsRealtime(duration);

        // フェードアウト
        for (float t = 0f; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            canvasGroup.alpha = 1f - t / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 0f;

        canvas.enabled = false;
        current = null;
    }
}
