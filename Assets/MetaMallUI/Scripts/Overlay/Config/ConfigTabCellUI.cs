public class ConfigTabCellUI : CellUIBase
{
    private ConfigType configType;

    public ConfigType ConfigType => configType;

    public void SetData(ConfigType type)
    {
        configType = type;
        Title.text = Mgr.Local.Get($"config-type-{type.ToString().ToLower()}");
    }
}
