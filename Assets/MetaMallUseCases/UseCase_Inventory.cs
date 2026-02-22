using System.Collections.Generic;
using UnityEngine;

public class UseCase_Inventory : MonoBehaviour
{
    private Main main;
    private readonly Dictionary<int, float> scrollPositions = new();

    public void Setup(Main main)
    {
        this.main = main;
        main.ui.OverlayStack.inventoryUI.Setup(OnBeforeSwitch, OnAfterSwitch, OnInventoryHide);
        main.ui.TopMenuUI.subMenuUI.selectInbentryMenuUI.button.SetOnClick(OnShowInventory);
    }

    public void OnShowInventory()
    {
        main.ui.OverlayStack.inventoryUI.Show();
    }

    private void OnBeforeSwitch(int oldIndex)
    {
        if (oldIndex < 0) return;
        scrollPositions[oldIndex] = main.ui.OverlayStack.inventoryUI.ScrollPosition;
    }

    private void OnAfterSwitch(int newIndex)
    {
        Mgr.Save.SessionData.inventoryTypeIndex = newIndex;

        if (scrollPositions.TryGetValue(newIndex, out float pos))
            main.ui.OverlayStack.inventoryUI.ScrollPosition = pos;
    }

    private void OnInventoryHide()
    {
        if (main.ui.OverlayStack.inventoryUI.SelectedType == RewardDisplayType.Use)
            Mgr.Save.RewardStockData.ClearNewUseItems();

        scrollPositions.Clear();
        UpdateBadge();
    }

    public void OnRefresh()
    {
        UpdateBadge();
    }

    public void UpdateBadge()
    {
        var badge = main.ui.TopMenuUI.subMenuUI.selectInbentryMenuUI.badge;
        if (badge != null)
            badge.SetVisible(Mgr.Save.RewardStockData.HasNewUseItem);
    }
}
