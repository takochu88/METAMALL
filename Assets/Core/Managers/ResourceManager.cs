using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    readonly Dictionary<string, Object> cache = new();

    /// <summary>
    /// Resources フォルダからアセットをロードする（キャッシュ付き）。
    /// key は Resources/ からの相対パス（拡張子なし）。
    /// </summary>
    public T Load<T>(string key) where T : Object
    {
        if (cache.TryGetValue(key, out var cached))
            return cached as T;

        var asset = Resources.Load<T>(key);
        if (asset != null)
            cache[key] = asset;

        return asset;
    }

    /// <summary>
    /// Prefab をロードしてインスタンス化し、指定コンポーネントを返す。
    /// </summary>
    public T Instantiate<T>(string key, Transform parent = null) where T : Component
    {
        var prefab = Load<GameObject>(key);
        if (prefab == null) return null;

        return Object.Instantiate(prefab, parent).GetComponent<T>();
    }

    /// <summary>
    /// キャッシュをクリアし、未使用アセットを解放する。
    /// </summary>
    public void ReleaseAll()
    {
        cache.Clear();
        Resources.UnloadUnusedAssets();
    }
}
