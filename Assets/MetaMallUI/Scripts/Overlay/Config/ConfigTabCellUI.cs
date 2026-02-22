using System;

public class ConfigTabCellUI : CellUIBase
{
    private ConfigType configType;
    private Action<ConfigType> onClick;

    public ConfigType ConfigType => configType;

    public void Setup(Action<ConfigType> onClick)
    {
        this.onClick = onClick;
        Button.SetOnClick(OnClick);
    }

    public void SetData(ConfigType type)
    {
        configType = type;
        Title.text = Mgr.Local.Get($"config-type-{type.ToString().ToLower()}");
    }

    private void OnClick()
    {
        onClick?.Invoke(configType);
    }
}
