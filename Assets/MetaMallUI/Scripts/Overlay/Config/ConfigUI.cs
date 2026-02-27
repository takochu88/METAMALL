using System;
using System.Collections.Generic;
using UnityEngine;

public class ConfigUI : OverlayUIBase
{
    [SerializeField] private MyScrollRect tabScroll;
    [SerializeField] private MyScrollRect listScroll;

    private int selectedIndex => Mgr.Save.SessionData.GetIndex(MenuType.Config);

    private readonly List<ConfigMaster> filteredMasters = new();

    private static readonly ConfigType[] AllTypes =
        (ConfigType[])Enum.GetValues(typeof(ConfigType));

    public ConfigType SelectedType => AllTypes[selectedIndex];

    public void Setup(Action<int> onSelectCell, Action<string, float> onValueChanged, Action<string> onButtonClicked)
    {
        tabScroll.InitSelectCell(onSelectCell);
        tabScroll.Init(AllTypes.Length, OnUpdateTab);

        listScroll.Init(cell =>
        {
            cell.Get<ConfigCellUI>().Setup(onValueChanged, onButtonClicked);
        });
    }

    /// <summary>指定インデックスのタブに切り替える（純粋な UI 更新）。</summary>
    public void SelectIndex(int index)
    {
        if (index < 0 || index >= AllTypes.Length) return;

        RefreshTabs();
        RefreshList();
    }

    private void RefreshTabs()
    {
        tabScroll.Init(AllTypes.Length, null, OnUpdateTab);
    }

    private void OnUpdateTab(int index, MyScrollCell cell)
    {
        var tab = (ConfigTabCellUI)cell;
        tab.SetData(AllTypes[index]);
        tab.SetSelected(index == selectedIndex);
    }

    private void RefreshList()
    {
        filteredMasters.Clear();
        var table = Mgr.Master.configTable;
        var selectedType = AllTypes[selectedIndex];

        for (int i = 0; i < table.Count; i++)
        {
            if (table[i].configType == selectedType)
                filteredMasters.Add(table[i]);
        }

        listScroll.Init(filteredMasters.Count, null, OnUpdateCell);
    }

    private void OnUpdateCell(int index, MyScrollCell cell)
    {
        var row = (ConfigCellUI)cell;
        var master = filteredMasters[index];
        float value = Mgr.Save.Config.GetValue(master);
        row.SetData(master, value);
    }

    protected override void OnAfterShow()
    {
        base.OnAfterShow();
        SelectIndex(selectedIndex);
    }

    public override void OnActivated()
    {
        base.OnActivated();
        RefreshList();
    }
}
