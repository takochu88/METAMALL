using System.Collections.Generic;

public class UseCase_Inventory : UseCase_MenuBase
{
    protected override MenuType MenuType => MenuType.Inventory;

    private readonly Dictionary<int, float> scrollPositions = new();
    private InventoryUI inventoryUI;

    public void Setup(Main main)
    {
        this.main = main;
        inventoryUI = main.ui.OverlayStack.inventoryUI;
        inventoryUI.Setup(OnSelectCell, OnInventoryHide);
        main.ui.TopMenuUI.subMenuUI.selectInventoryMenuUI.button.SetOnClick(OnShowInventory);
        Mgr.Save.RewardStockData.OnNewUseItemsChanged += OnNewUseItemsChanged;
    }

    private void OnNewUseItemsChanged()
    {
        UpdateBadge();
        inventoryUI.RefreshCellBadges();
    }

    public void OnShowInventory()
    {
        main.ui.OverlayStack.inventoryUI.Show();
    }

    protected override void OnSelectCell(int newIndex)
    {
        // 旧カテゴリのスクロール位置を保存
        int oldIndex = SelectedIndex;
        scrollPositions[oldIndex] = inventoryUI.ScrollPosition;

        // SessionData 更新
        base.OnSelectCell(newIndex);

        // UI 更新
        inventoryUI.SelectIndex(newIndex);

        // 新カテゴリのスクロール位置を復元
        if (scrollPositions.TryGetValue(newIndex, out float pos))
            inventoryUI.ScrollPosition = pos;
    }

    private void OnInventoryHide()
    {
        var types = RewardDisplayTypeExtensions.GetInventoryTypes();
        if (SelectedIndex >= 0 && SelectedIndex < types.Length && types[SelectedIndex] == RewardDisplayType.Use)
            Mgr.Save.RewardStockData.ClearNewUseItems();

        scrollPositions.Clear();
    }

    public void OnRefresh()
    {
        UpdateBadge();
    }

    public void UpdateBadge()
    {
        var badge = main.ui.TopMenuUI.subMenuUI.selectInventoryMenuUI.badge;
        if (badge != null)
            badge.SetVisible(Mgr.Save.RewardStockData.HasNewUseItem);
    }
}
