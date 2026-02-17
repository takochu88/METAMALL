using TMPro;
using UnityEngine;

public class DebugPanelUI : OverlayUIBase
{
    [Header("固定コマンド")]
    [SerializeField] DebugRewardRowUI rewardRow;
    [SerializeField] DebugFlagRowUI flagRow;

    [Header("フィルタ")]
    [SerializeField] TMP_InputField filterInput;
    [SerializeField] MyScrollRect scroll;

    Main main;

    public void Setup(Main main)
    {
        this.main = main;
        rewardRow.Setup(main);
        flagRow.Setup();
    }

    protected override void OnBeforeShow()
    {
        rewardRow.SetDefault();
        filterInput.text = "";
        Refresh("");
    }

    protected override void OnAfterShow()
    {
        filterInput.onValueChanged.AddListener(OnFilterChanged);
        filterInput.ActivateInputField();
    }

    protected override void OnAfterHide()
    {
        filterInput.onValueChanged.RemoveListener(OnFilterChanged);
    }

    void OnFilterChanged(string value)
    {
        Refresh(value);
    }

    void Refresh(string filter)
    {
        var list = DebugCommandList.GetFiltered(filter);
        scroll.Init(
            list.Count,
            cell => cell.Get<DebugCommandRowUI>().Init(OnCommandSelected),
            (index, cell) => cell.Get<DebugCommandRowUI>().Setup(list[index], OnCommandSelected)
        );
    }

    void OnCommandSelected(DebugCommand command)
    {
        command.Execute?.Invoke(main);
        Hide();
    }
}
