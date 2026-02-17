using System;

[Serializable]
public abstract class MasterBase
{
    public string key;
    public int index;

    [NonSerialized] private string _cachedName;
    [NonSerialized] private string _cachedDesc;
    [NonSerialized] private bool _nameCached;
    [NonSerialized] private bool _descCached;

    public string Name => GetCached(ref _nameCached, ref _cachedName, GetNameKey);
    public string Desc => GetCached(ref _descCached, ref _cachedDesc, GetDescKey);

    protected virtual string GetNameKey() => "name-" + key;
    protected virtual string GetDescKey() => "desc-" + key;

    private static string GetCached(ref bool cached, ref string cache, Func<string> buildKey)
    {
        if (!cached)
        {
            string k = buildKey?.Invoke();
            // null/empty 保険（必要ならここで Debug.LogWarning してもOK）
            cache = string.IsNullOrEmpty(k) ? string.Empty : Mgr.Local.Get(k);
            cached = true;
        }
        return cache ?? string.Empty;
    }

    /// <summary> key を差し替えた/ローカライズをリロードした等で取り直したい時 </summary>
    public void InvalidateLocalizedCache()
    {
        _nameCached = false;
        _descCached = false;
        _cachedName = null;
        _cachedDesc = null;
    }
}