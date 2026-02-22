using System;
using UnityEngine;

[Serializable]
public abstract class FighterMaster : MasterBase
{
    public Dimension dimension;
    public int[] talents;
    public int GetTalent(TalentType type) => talents[(int)type];
    public void SetTalent(TalentType type, int value) => talents[(int)type] = value;

    [NonSerialized] private Sprite _cachedIcon;
    [NonSerialized] private Sprite _cachedFullBody;
    [NonSerialized] private bool _iconCached;
    [NonSerialized] private bool _fullBodyCached;

    public Sprite Icon
    {
        get
        {
            if (!_iconCached)
            {
                _cachedIcon = Mgr.Resource.Load<Sprite>("Fighters/Icon/icon-" + key);
                _iconCached = true;
            }
            return _cachedIcon;
        }
    }

    public Sprite FullBody
    {
        get
        {
            if (!_fullBodyCached)
            {
                _cachedFullBody = Mgr.Resource.Load<Sprite>("Fighters/FullBody/full-body-" + key);
                _fullBodyCached = true;
            }
            return _cachedFullBody;
        }
    }
}
