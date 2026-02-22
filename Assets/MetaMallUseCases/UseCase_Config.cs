using UnityEngine;

public class UseCase_Config : MonoBehaviour
{
    private Main main;

    public void Setup(Main main)
    {
        this.main = main;
        main.ui.OverlayStack.configUI.Setup(OnTypeSelected, OnValueChanged, OnButtonClicked);
    }

    public void ShowConfig()
    {
        main.ui.OverlayStack.configUI.Show();
    }

    private void OnTypeSelected(int typeIndex)
    {
        Mgr.Save.SessionData.configTypeIndex = typeIndex;
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
