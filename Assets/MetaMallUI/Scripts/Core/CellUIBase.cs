using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class CellUIBase : MyScrollCell
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private BadgeUI badgeUI;
    [SerializeField] private Image background;

    public static readonly Color NormalColor = new Color(1f, 1f, 1f, 0.5f);
    public static readonly Color SelectedColor = new Color(0f, 0.75f, 1f, 0.5f);

    protected TMP_Text Title => title;
    protected BadgeUI BadgeUI => badgeUI;
    protected Image Background => background;

    public void SetSelected(bool selected)
    {
        if (background != null)
            background.color = selected ? SelectedColor : NormalColor;
    }

    public void SetBadgeVisible(bool visible)
    {
        if (badgeUI != null)
            badgeUI.gameObject.SetActiveIfChanged(visible);
    }

}
