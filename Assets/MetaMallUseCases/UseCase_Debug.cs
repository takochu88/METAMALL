using UnityEngine;
using UnityEngine.InputSystem;

public class UseCase_Debug : MonoBehaviour
{
    Main main;
    DebugPanelUI debugPanelUI;

    public void Setup(Main main)
    {
        this.main = main;
        debugPanelUI = main.ui.OverlayStack.debugPanelUI;
        debugPanelUI.Setup(main);
    }

    public void OnUpdate()
    {
        if (filterInput_IsFocused()) return;

        if (Keyboard.current?.dKey.wasPressedThisFrame == true)
        {
            if (debugPanelUI.IsShown)
                debugPanelUI.Hide();
            else
                debugPanelUI.Show();
        }
    }

    bool filterInput_IsFocused()
    {
        var selected = UnityEngine.EventSystems.EventSystem.current?.currentSelectedGameObject;
        if (selected == null) return false;
        return selected.GetComponent<TMPro.TMP_InputField>() != null;
    }
}
