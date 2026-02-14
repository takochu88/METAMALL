using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string ConfigKey = "ConfigData";

    public ConfigData Config { get; private set; } = new ConfigData();

    public void Load()
    {
        var json = PlayerPrefs.GetString(ConfigKey, "");
        if (!string.IsNullOrEmpty(json))
        {
            Config = JsonUtility.FromJson<ConfigData>(json);
        }
    }

    public void Save()
    {
        var json = JsonUtility.ToJson(Config);
        PlayerPrefs.SetString(ConfigKey, json);
        PlayerPrefs.Save();
    }

    public Language GetLanguage() => Config.language;

    public void SetLanguage(Language language)
    {
        Config.language = language;
        Save();
    }
}
