using System.Collections.Generic;
using UnityEngine;

public class LocalManager : MonoBehaviour
{
    [SerializeField] private List<LocalizationTable> localizationTables = new List<LocalizationTable>();

    private Dictionary<string, LocalizationMaster> _map;

    public void Init()
    {
        _map = new Dictionary<string, LocalizationMaster>();

        foreach (var table in localizationTables)
        {
            if (table == null) continue;
            foreach (var row in table)
            {
                if (!string.IsNullOrEmpty(row.key))
                {
                    _map[row.key] = row;
                }
            }
        }
    }

    /// <summary>
    /// キーに対応する翻訳テキストを返す。
    /// </summary>
    public string Get(string key)
    {
        if (_map == null) Init();

        if (!_map.TryGetValue(key, out var master))
        {
            Debug.LogWarning($"[LocalManager] キーが見つかりません: {key}");
            return key;
        }

        return GetText(master);
    }

    /// <summary>
    /// キーに対応する翻訳テキストに引数を埋め込んで返す。
    /// 本文中の {0}{1}{2}... が args に置換される。
    /// </summary>
    public string Format(string key, params object[] args)
    {
        var text = Get(key);
        return string.Format(text, args);
    }

    private string GetText(LocalizationMaster master)
    {
        var lang = Mgr.Save.GetLanguage();
        return lang switch
        {
            Language.Ja => master.ja,
            Language.En => master.en,
            _ => master.ja,
        };
    }
}
