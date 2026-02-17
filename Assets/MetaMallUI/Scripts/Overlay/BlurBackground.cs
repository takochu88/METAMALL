using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Show時に1回だけ画面をキャプチャし、ガウシアンブラーを適用してRawImageに表示する。
/// </summary>
public class BlurBackground : MonoBehaviour
{
    [SerializeField] RawImage rawImage;
    [SerializeField] Shader blurShader;
    [SerializeField, Range(1, 4)] int downSample = 2;
    [SerializeField, Range(0f, 1000f)] float blur = 100f;
    [SerializeField, Range(0.1f, 4f)] float offset = 1f;

    Material blurMaterial;
    RenderTexture blurredTexture;

    static readonly int WeightsId = Shader.PropertyToID("_Weights");
    static readonly int OffsetId = Shader.PropertyToID("_Offset");

    public void Capture(Action onComplete = null)
    {
        StartCoroutine(CaptureRoutine(onComplete));
    }

    IEnumerator CaptureRoutine(Action onComplete)
    {
        // Darkが描画されたフレームを待つ
        yield return new WaitForEndOfFrame();

        EnsureMaterial();

        int w = Screen.width / downSample;
        int h = Screen.height / downSample;

        // 画面キャプチャ
        var screen = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
        ScreenCapture.CaptureScreenshotIntoRenderTexture(screen);

        // ダウンサンプル（Y反転補正）
        var downsampled = RenderTexture.GetTemporary(w, h, 0);
        if (SystemInfo.graphicsUVStartsAtTop)
            Graphics.Blit(screen, downsampled, new Vector2(1, -1), new Vector2(0, 1));
        else
            Graphics.Blit(screen, downsampled);
        RenderTexture.ReleaseTemporary(screen);

        // ガウス重み計算
        UpdateWeights();

        // 水平ブラー
        var temp = RenderTexture.GetTemporary(w, h, 0);
        blurMaterial.SetVector(OffsetId, new Vector4(offset / w, 0, 0, 0));
        Graphics.Blit(downsampled, temp, blurMaterial);

        // 垂直ブラー
        blurMaterial.SetVector(OffsetId, new Vector4(0, offset / h, 0, 0));
        Graphics.Blit(temp, downsampled, blurMaterial);

        RenderTexture.ReleaseTemporary(temp);

        // 結果を保持してRawImageに設定
        Release();
        blurredTexture = downsampled;
        rawImage.texture = blurredTexture;
        rawImage.uvRect = CalcUvRect();
        rawImage.enabled = true;

        onComplete?.Invoke();
    }

    public void Release()
    {
        if (blurredTexture != null)
        {
            if (rawImage != null)
            {
                rawImage.texture = null;
                rawImage.enabled = false;
            }

            RenderTexture.ReleaseTemporary(blurredTexture);
            blurredTexture = null;
        }
    }

    Rect CalcUvRect()
    {
        var corners = new Vector3[4];
        rawImage.rectTransform.GetWorldCorners(corners);

        var canvas = rawImage.canvas;
        var cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        Vector2 min = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
        Vector2 max = RectTransformUtility.WorldToScreenPoint(cam, corners[2]);

        float sw = Screen.width;
        float sh = Screen.height;
        return new Rect(min.x / sw, min.y / sh, (max.x - min.x) / sw, (max.y - min.y) / sh);
    }

    void EnsureMaterial()
    {
        if (blurMaterial == null)
            blurMaterial = new Material(blurShader);
    }

    void UpdateWeights()
    {
        float total = 0f;
        float d = blur * blur * 0.001f;
        var weights = new float[10];

        for (int i = 0; i < weights.Length; i++)
        {
            float x = i * 1.0f;
            float w = Mathf.Exp(-0.5f * x * x / d);
            weights[i] = w;
            total += i == 0 ? w : w * 2f;
        }

        for (int i = 0; i < weights.Length; i++)
            weights[i] /= total;

        blurMaterial.SetFloatArray(WeightsId, weights);
    }

    void OnDisable() => Release();

    void OnDestroy()
    {
        Release();
        if (blurMaterial != null)
            DestroyImmediate(blurMaterial);
    }
}
