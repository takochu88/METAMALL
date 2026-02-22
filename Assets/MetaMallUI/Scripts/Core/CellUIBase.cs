using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class CellUIBase : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private MyButton button;
    [SerializeField] private BadgeUI badgeUI;
    [SerializeField] private Image background;

    public static readonly Color NormalColor = Color.white;
    public static readonly Color SelectedColor = Color.deepSkyBlue;

    protected TMP_Text Title => title;
    protected MyButton Button => button;
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
