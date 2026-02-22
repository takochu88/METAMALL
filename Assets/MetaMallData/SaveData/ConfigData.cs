using System;
using System.Collections.Generic;

[Serializable]
public class ConfigData
{
    public Language language = Language.Ja;
    public List<ConfigEntry> entries = new();

    [NonSerialized] private Dictionary<string, float> _cache;

    private Dictionary<string, float> Cache
    {
        get
        {
            if (_cache != null) return _cache;
            _cache = new Dictionary<string, float>();
            foreach (var e in entries)
                _cache[e.key] = e.value;
            return _cache;
        }
    }

    public float GetValue(ConfigMaster master)
    {
        return Cache.TryGetValue(master.key, out var v) ? v : master.defaultValue;
    }

    public void SetValue(string key, float value)
    {
        Cache[key] = value;

        for (int i = 0; i < entries.Count; i++)
        {
            if (entries[i].key == key)
            {
                entries[i] = new ConfigEntry { key = key, value = value };
                return;
            }
        }
        entries.Add(new ConfigEntry { key = key, value = value });
    }

    [Serializable]
    public struct ConfigEntry
    {
        public string key;
        public float value;
    }
}
