using UnityEngine;

public class OverlayUIBase : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    public bool IsVisible => gameObject.activeSelf;

    protected virtual void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }
}
