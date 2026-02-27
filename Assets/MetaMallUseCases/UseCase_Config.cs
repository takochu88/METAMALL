using UnityEngine;

public class UseCase_Config : UseCase_MenuBase
{
    protected override MenuType MenuType => MenuType.Config;

    public void Setup(Main main)
    {
        this.main = main;
        main.ui.OverlayStack.configUI.Setup(OnSelectCell, OnValueChanged, OnButtonClicked);
    }

    public void ShowConfig()
    {
        main.ui.OverlayStack.configUI.Show();
    }

    protected override void OnSelectCell(int newIndex)
    {
        base.OnSelectCell(newIndex);
        main.ui.OverlayStack.configUI.SelectIndex(newIndex);
    }

    private void OnValueChanged(string key, float value)
    {
        Mgr.Save.Config.SetValue(key, value);
        Mgr.Save.Save();
    }

    private void OnButtonClicked(string key)
    {
        Debug.Log($"[Config] Button clicked: {key}");
    }
}
