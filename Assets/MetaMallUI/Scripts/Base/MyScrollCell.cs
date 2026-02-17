using UnityEngine;

/// <summary>
/// MyScrollRect が管理するセル。プレハブに付けるか、自動付与される。
/// Get&lt;T&gt;() で Row コンポーネントをキャッシュ付きで取得できる。
/// </summary>
public class MyScrollCell : MonoBehaviour
{
    public int Index { get; internal set; } = -1;
    public RectTransform RectTransform { get; private set; }

    Component cachedRow;

    void Awake()
    {
        RectTransform = (RectTransform)transform;
    }

    /// <summary>
    /// Row コンポーネントをキャッシュ付きで取得。
    /// 初回のみ GetComponent、2回目以降はキャッシュを返す。
    /// </summary>
    public T Get<T>() where T : Component
    {
        if (cachedRow is T t) return t;
        t = GetComponent<T>();
        cachedRow = t;
        return t;
    }
}
