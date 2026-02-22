using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetRewardUI : OverlayUIBase
{
    [SerializeField] RewardListUI rewardListUI;
    [SerializeField] private MyButton okButton;
    [SerializeField] private MyButton cancelButton;
    [SerializeField] private TMP_Text stateText;

    private List<RewardEntry> rewards;
    private List<RewardEntry> overflows;
    private RewardDisplayMode displayMode = RewardDisplayMode.OneByOne;
    private RewardMergeMode mergeMode = RewardMergeMode.PreMerge;
    private GetRewardType rewardType;
    public bool IsForceGet { get; private set; }

    public void Setup()
    {
        okButton.SetOnClick(OnClickOk);
        cancelButton.SetOnClick(OnClickCancel);
    }

    public void SetRewards(List<RewardEntry> rewards,
        GetRewardType getRewardType,
        RewardDisplayMode display = RewardDisplayMode.OneByOne,
        RewardMergeMode merge = RewardMergeMode.PreMerge)
    {
        this.rewards = new List<RewardEntry>(rewards);
        this.displayMode = display;
        this.mergeMode = merge;
        stateText.text = getRewardType.GetTitle();
        rewardListUI.background.color = getRewardType.GetColor();
        rewardType = getRewardType;
        // ボタンはアニメーション完了後に表示するため、初期状態では隠す
        okButton.gameObject.SetActiveIfChanged(false);
        cancelButton.gameObject.SetActiveIfChanged(false);
        CloseScreen.gameObject.SetActiveIfChanged(false);
    }

    protected override void OnAfterShow()
    {
        HideCloseLabel();

        rewardListUI.Setup(rewards, displayMode, mergeMode,
            onComplete: () =>
            {
                bool isOverflow = rewardType == GetRewardType.OverFlow;
                okButton.gameObject.SetActiveIfChanged(isOverflow);
                cancelButton.gameObject.SetActiveIfChanged(isOverflow);
                CloseScreen.gameObject.SetActiveIfChanged(!isOverflow);
                if (!isOverflow) ShowCloseLabel();
            });
    }

    private void OnClickOk()
    {
        IsForceGet = true;
        Hide();
    }

    private void OnClickCancel()
    {
        IsForceGet = false;
        Hide();
    }
}
