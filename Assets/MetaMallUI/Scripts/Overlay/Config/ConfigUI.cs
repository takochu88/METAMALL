using System;
using System.Collections.Generic;
using UnityEngine;

public class ConfigUI : OverlayUIBase
{
    [SerializeField] private MyScrollRect tabScroll;
    [SerializeField] private MyScrollRect listScroll;

    private bool isSetup;
    private int selectedIndex;
    private Action<int> onTypeSelected;
    private Action<string, float> onValueChanged;
    private Action<string> onButtonClicked;

    private readonly List<ConfigMaster> filteredMasters = new();

    private static readonly ConfigType[] AllTypes =
        (ConfigType[])Enum.GetValues(typeof(ConfigType));

    public ConfigType SelectedType => AllTypes[selectedIndex];

    public void Setup(Action<int> onTypeSelected, Action<string, float> onValueChanged, Action<string> onButtonClicked)
    {
        if (isSetup) return;
        isSetup = true;
        this.onTypeSelected = onTypeSelected;
        this.onValueChanged = onValueChanged;
        this.onButtonClicked = onButtonClicked;
    }

    public void SelectIndex(int index)
    {
        if (index < 0 || index >= AllTypes.Length) return;

        selectedIndex = index;
        onTypeSelected?.Invoke(index);
        RefreshTabs();
        RefreshList();
    }

    private void RefreshTabs()
    {
        tabScroll.Init(AllTypes.Length, OnInitTab, OnUpdateTab);
    }

    private void OnInitTab(MyScrollCell cell)
    {
        var tab = cell.Get<ConfigTabCellUI>();
        tab.Setup(OnTabClicked);
    }

    private void OnUpdateTab(int index, MyScrollCell cell)
    {
        var tab = cell.Get<ConfigTabCellUI>();
        tab.SetData(AllTypes[index]);
        tab.SetSelected(index == selectedIndex);
    }

    private void OnTabClicked(ConfigType type)
    {
        for (int i = 0; i < AllTypes.Length; i++)
        {
            if (AllTypes[i] == type)
            {
                SelectIndex(i);
                return;
            }
        }
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

        listScroll.Init(filteredMasters.Count, OnInitCell, OnUpdateCell);
    }

    private void OnInitCell(MyScrollCell cell)
    {
        var row = cell.Get<ConfigCellUI>();
        row.Setup(onValueChanged, onButtonClicked);
    }

    private void OnUpdateCell(int index, MyScrollCell cell)
    {
        var row = cell.Get<ConfigCellUI>();
        var master = filteredMasters[index];
        float value = Mgr.Save.Config.GetValue(master);
        row.SetData(master, value);
    }

    protected override void OnAfterShow()
    {
        base.OnAfterShow();
        int savedIndex = Mgr.Save.SessionData.configTypeIndex;
        SelectIndex(savedIndex);
    }

    public override void OnActivated()
    {
        base.OnActivated();
        RefreshList();
    }
}
