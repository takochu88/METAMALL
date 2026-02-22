using System;
using UnityEngine;

public abstract class MenuBase
{
    public string key;
    public MenuType menuType;
    public FlagType unlockFlag;

    [NonSerialized] private string _cachedName;
    [NonSerialized] private Sprite _cachedIcon;
    [NonSerialized] private bool _nameCached;
    [NonSerialized] private bool _iconCached;

    public string Name
    {
        get
        {
            if (!_nameCached)
            {
                _cachedName = Mgr.Local.Get("name-" + key);
                _nameCached = true;
            }
            return _cachedName ?? string.Empty;
        }
    }

    public Sprite IconSprite
    {
        get
        {
            if (!_iconCached)
            {
                _cachedIcon = Mgr.Resource.Load<Sprite>("Icons/" + key);
                _iconCached = true;
            }
            return _cachedIcon;
        }
    }

    public void InvalidateCache()
    {
        _nameCached = false;
        _iconCached = false;
        _cachedName = null;
        _cachedIcon = null;
    }
}
