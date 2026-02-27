using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : OverlayUIBase
{
    [SerializeField] private MyScrollRect scroll;
    [SerializeField] private RewardListUI rewardListUI;
    [SerializeField] private TMP_Text emptyText;

    private int selectedIndex => Mgr.Save.SessionData.GetIndex(MenuType.Inventory);
    private Action onHide;

    public void Setup(Action<int> onSelectCell, Action onHide = null)
    {
        this.onHide = onHide;
        scroll.InitSelectCell(onSelectCell);
        var types = RewardDisplayTypeExtensions.GetInventoryTypes();
        scroll.Init(types.Length, OnUpdateCell);
    }

    /// <summary>RewardListUI のスクロール位置（0〜1）。</summary>
    public float ScrollPosition
    {
        get => rewardListUI.ScrollPosition;
        set => rewardListUI.ScrollPosition = value;
    }

    /// <summary>指定インデックスの表示に切り替える（純粋な UI 更新）。</summary>
    public void SelectIndex(int index)
    {
        var types = RewardDisplayTypeExtensions.GetInventoryTypes();
        if (index < 0 || index >= types.Length) return;

        scroll.UpdateSelection(index);
        ShowRewardList(types[index]);
    }

    public void RefreshCellBadges()
    {
        var types = RewardDisplayTypeExtensions.GetInventoryTypes();
        for (int i = 0; i < types.Length; i++)
        {
            var cell = scroll.FindVisibleCell(i);
            if (cell != null) ((InventoryCellUI)cell).UpdateData();
        }
    }

    private void RefreshCell()
    {
        var types = RewardDisplayTypeExtensions.GetInventoryTypes();
        scroll.Init(types.Length, null, OnUpdateCell);
    }

    private void OnUpdateCell(int index, MyScrollCell _cell)
    {
        var cell = (InventoryCellUI)_cell;
        cell.UpdateData();
        cell.SetSelected(index == selectedIndex);
    }

    private void ShowRewardList(RewardDisplayType type)
    {
        var masters = Mgr.Master.GetRewardsByDisplayType(type);
        var stock = Mgr.Save.RewardStockData.GetStock(type);

        var entries = new List<RewardEntry>();
        foreach (var reward in masters)
        {
            if (!stock.ShouldDisplay(reward)) continue;
            int amount = stock.GetAmount(reward.rewardId);
            entries.Add(new RewardEntry(reward.rewardId, amount));
        }

        if (entries.Count == 0)
        {
            SetRewardListVisible(false);
            SetEmptyVisible(true);
        }
        else
        {
            SetEmptyVisible(false);
            SetRewardListVisible(true);
            rewardListUI.Setup(entries, oneLineArrangeType: RewardListOneLineArrangeType.Left, canUse: true);
        }
    }

    private void SetRewardListVisible(bool visible)
    {
        rewardListUI.gameObject.SetActiveIfChanged(visible);
    }

    private void SetEmptyVisible(bool visible)
    {
        if (emptyText != null)
            emptyText.gameObject.SetActiveIfChanged(visible);
    }

    protected override void OnAfterShow()
    {
        base.OnAfterShow();
        SelectIndex(selectedIndex);
    }

    public override void OnActivated()
    {
        base.OnActivated();
        RefreshCell();
    }

    protected override void OnAfterHide()
    {
        base.OnAfterHide();
        onHide?.Invoke();
    }
}
