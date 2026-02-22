using System;
using TMPro;
using UnityEngine;

public class RewardUseUI : OverlayUIBase
{
    [SerializeField] private RewardIconUI rewardIcon;
    [SerializeField] private TMP_Text nameLabel;
    [SerializeField] private TMP_Text amountLabel;
    [SerializeField] private NumberSelectUI numberSelect;
    [SerializeField] private MyButton confirmButton;

    private Action<int, int> onConfirmUseReward;
    private int rewardId;
    private int stock;

    private void Awake()
    {
        if (confirmButton != null) confirmButton.SetOnClick(OnConfirm);
        rewardIcon.SetSize(160);
    }

    public void Setup(Action<int, int> onConfirmUseReward)
    {
        this.onConfirmUseReward = onConfirmUseReward;
    }

    public void SetData(int rewardId)
    {
        this.rewardId = rewardId;

        if (!Mgr.Master.TryGetReward(rewardId, out var reward)) return;

        stock = UseCase_Reward.GetUseMax(rewardId);
        rewardIcon.SetData(reward, RewardIconDisplayType.StockAmount, stock);
        nameLabel.text = reward.Name;
        amountLabel.text = stock.ToString();
        numberSelect.Setup(1, stock);
    }

    private void OnConfirm()
    {
        onConfirmUseReward(rewardId, numberSelect.Value);
    }
}
