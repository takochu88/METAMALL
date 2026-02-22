using System;

[Serializable]
public class ConfigMaster : MasterBase
{
    public ConfigOptionType configOptionType;
    public ConfigType configType;
    public float defaultValue;
    public float min;
    public float max = 1f;
    public string[] choiceKeys;
}
