using UnityEngine;

public class BadgeUI : MonoBehaviour
{
    [SerializeField] float minScale = 0.8f;
    [SerializeField] float maxScale = 1.2f;
    [SerializeField] float speed = 3f;

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
}
