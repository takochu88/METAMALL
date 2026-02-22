using System;
using UnityEngine;

[Serializable]
public abstract class RewardBase
{
    public const int Multiplier = 10000;
    public const int DefaultMaxStock = 999999999;

    public string key;
    public int rewardId;
    public bool showWhenEmpty;
    public int maxStock = -1;
    public RewardIncreaseMethodType increaseMethodType;
    public Dimension dimension;
    public int MaxStock => maxStock < 0 ? DefaultMaxStock : maxStock;
    public RewardType Type => (RewardType)(rewardId / Multiplier);
    public int Id => rewardId % Multiplier;
    public abstract RewardDisplayType DisplayType { get; }

    [NonSerialized] private string _cachedName;
    [NonSerialized] private string _cachedDesc;
    [NonSerialized] private Sprite _cachedIcon;
    [NonSerialized] private bool _nameCached;
    [NonSerialized] private bool _descCached;
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

    public string Desc
    {
        get
        {
            if (!_descCached)
            {
                _cachedDesc = Mgr.Local.Get("desc-" + key);
                _descCached = true;
            }
            return _cachedDesc ?? string.Empty;
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
        _descCached = false;
        _iconCached = false;
        _cachedName = null;
        _cachedDesc = null;
        _cachedIcon = null;
    }
}
