public class InventoryCellUI : CellUIBase
{
    public void UpdateData()
    {
        RewardDisplayType type = (RewardDisplayType)Index;
        Title.text = type.ToLocalizedName();
        SetBadgeVisible(type == RewardDisplayType.Use && Mgr.Save.RewardStockData.HasNewUseItem);
    }
}
