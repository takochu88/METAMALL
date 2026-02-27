using UnityEngine;

public class BadgeUI : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.2f;
    [SerializeField] private float speed = 3f;

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        float s = Mathf.Lerp(minScale, maxScale, t);
        transform.localScale = new Vector3(s, s, s);
    }

    void OnDisable()
    {
        transform.localScale = Vector3.one;
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActiveIfChanged(visible);
    }

    public void AdjustSize(float size)
    {
        rect.sizeDelta = new Vector2(size, size);
    }
}
