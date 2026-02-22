using UnityEngine;

/// <summary>
/// アタッチした RectTransform を Screen.safeArea に合わせて調整する。
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect lastSafeArea;

    private void Awake()
    {
        rectTransform = (RectTransform)transform;
    }

    private void OnEnable()
    {
        ApplySafeArea();
    }

    private void Update()
    {
        if (lastSafeArea != Screen.safeArea)
            ApplySafeArea();
    }

    private void ApplySafeArea()
    {
        var safeArea = Screen.safeArea;
        lastSafeArea = safeArea;

        var canvas = GetComponentInParent<Canvas>().rootCanvas;
        var canvasRT = (RectTransform)canvas.transform;
        var canvasSize = canvasRT.sizeDelta;

        // Canvas のスケーリングを考慮して正規化
        float scaleX = canvasSize.x / Screen.width;
        float scaleY = canvasSize.y / Screen.height;

        var anchorMin = new Vector2(safeArea.xMin * scaleX / canvasSize.x,
                                    safeArea.yMin * scaleY / canvasSize.y);
        var anchorMax = new Vector2(safeArea.xMax * scaleX / canvasSize.x,
                                    safeArea.yMax * scaleY / canvasSize.y);

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}
