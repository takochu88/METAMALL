using System;

public class InventoryCellUI : CellUIBase
{
    private RewardDisplayType displayType;
    private Action<RewardDisplayType> onClick;

    public RewardDisplayType DisplayType => displayType;

    public void Setup(Action<RewardDisplayType> onClick)
    {
        this.onClick = onClick;
        Button.SetOnClick(OnClick);
    }

    public void SetData(RewardDisplayType type)
    {
        displayType = type;
        Title.text = type.ToLocalizedName();
        SetBadgeVisible(type == RewardDisplayType.Use && Mgr.Save.RewardStockData.HasNewUseItem);
    }

    private void OnClick()
    {
        onClick?.Invoke(displayType);
    }
}
