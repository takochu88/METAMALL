using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : OverlayUIBase
{
    [SerializeField] private MyScrollRect scroll;
    [SerializeField] private RewardListUI rewardListUI;
    [SerializeField] private TMP_Text emptyText;

    private bool isSetup;
    private int selectedIndex = -1;
    private Action<int> onBeforeSwitch;
    private Action<int> onAfterSwitch;
    private Action onHide;

    public void Setup(Action<int> onBeforeSwitch, Action<int> onAfterSwitch, Action onHide = null)
    {
        if (isSetup) return;
        isSetup = true;
        this.onBeforeSwitch = onBeforeSwitch;
        this.onAfterSwitch = onAfterSwitch;
        this.onHide = onHide;
    }

    public RewardDisplayType SelectedType
    {
        get
        {
            var types = RewardDisplayTypeExtensions.GetInventoryTypes();
            return (selectedIndex >= 0 && selectedIndex < types.Length)
                ? types[selectedIndex]
                : RewardDisplayType.Currency;
        }
    }

    /// <summary>RewardListUI のスクロール位置（0〜1）。</summary>
    public float ScrollPosition
    {
        get => rewardListUI.ScrollPosition;
        set => rewardListUI.ScrollPosition = value;
    }

    public void SelectIndex(int index)
    {
        var types = RewardDisplayTypeExtensions.GetInventoryTypes();
        if (index < 0 || index >= types.Length) return;

        // 旧カテゴリのスクロール位置を保存する機会を与える
        onBeforeSwitch?.Invoke(selectedIndex);

        selectedIndex = index;
        RefreshRows();
        ShowRewardList(types[index]);

        // 新カテゴリのスクロール位置を復元する機会を与える
        onAfterSwitch?.Invoke(index);
    }

    private void RefreshRows()
    {
        var types = RewardDisplayTypeExtensions.GetInventoryTypes();
        scroll.Init(types.Length, OnInitCell, OnUpdateCell);
    }

    private void OnInitCell(MyScrollCell cell)
    {
        var row = cell.Get<InventoryCellUI>();
        row.Setup(OnRowClicked);
    }

    private void OnUpdateCell(int index, MyScrollCell cell)
    {
        var types = RewardDisplayTypeExtensions.GetInventoryTypes();
        var row = cell.Get<InventoryCellUI>();
        row.SetData(types[index]);
        row.SetSelected(index == selectedIndex);
    }

    private void OnRowClicked(RewardDisplayType type)
    {
        var types = RewardDisplayTypeExtensions.GetInventoryTypes();
        for (int i = 0; i < types.Length; i++)
        {
            if (types[i] == type)
            {
                SelectIndex(i);
                return;
            }
        }
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
        int savedIndex = Mgr.Save.SessionData.inventoryTypeIndex;
        SelectIndex(savedIndex);
    }

    public override void OnActivated()
    {
        base.OnActivated();
        RefreshRows();
    }

    protected override void OnAfterHide()
    {
        base.OnAfterHide();
        selectedIndex = -1;
        onHide?.Invoke();
    }
}
