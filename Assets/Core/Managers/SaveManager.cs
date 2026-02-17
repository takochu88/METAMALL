using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string ConfigKey = "ConfigData";

    public ConfigData Config { get; private set; } = new ConfigData();
    public FlagData FlagData { get; private set; } = new FlagData();
    public AllCharacterData AllCharacterData { get; private set; } = new AllCharacterData();
    public RewardStock Stock { get; private set; } = new RewardStock();

    public void Load()
    {
        AllCharacterData.Init(Mgr.Master.characterTable);
        return;

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
