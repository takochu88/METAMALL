using System;
using UnityEngine;

public class SelectMenuRowUI : MonoBehaviour
{
    public MyButton button;
    public BadgeUI badge;

    private int index;
    private Action<int> onSelectMenu;
    private Action onSelectSubMenu;

    public void SetupMain(int index, Action<int> onSelectMenu)
    {
        this.index = index;
        this.onSelectMenu = onSelectMenu;
        button.SetOnClick(OnClickMain);
    }

    public void SetupSub(Action onSelectSubMenu)
    {
        this.onSelectSubMenu = onSelectSubMenu;
        button.SetOnClick(OnClickSub);
    }
    
    public void SetVisible(bool visible)
    {
        gameObject.SetActiveIfChanged(visible);
    }

    private void OnClickMain()
    {
        onSelectMenu?.Invoke(index);
    }

    private void OnClickSub()
    {
        onSelectSubMenu?.Invoke();
    }
}
