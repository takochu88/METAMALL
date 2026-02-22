using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectMenuRowUI : MonoBehaviour
{
    [SerializeField] private MenuType menuType;
    [SerializeField] private SelectMenuDisplayType displayType;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text titleText;
    public MyButton button;
    public BadgeUI badge;

    private int index;
    private Action<int> onSelectMenu;
    private Action onSelectSubMenu;

    private void Awake()
    {
        if (Mgr.Master.TryGetMenuBase(menuType, out var menu))
        {
            icon.sprite = menu.IconSprite;
            titleText.text = menu.Name;
            icon.enabled = displayType == SelectMenuDisplayType.Icon ||
                           displayType == SelectMenuDisplayType.NameAndIcon;
            titleText.enabled = displayType == SelectMenuDisplayType.Name ||
                                displayType == SelectMenuDisplayType.NameAndIcon;
        }
        else
        {
            icon.enabled = false;
            titleText.enabled = false;
        }
    }

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
