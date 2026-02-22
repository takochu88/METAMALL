using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardDetailUI : OverlayUIBase
{
    [SerializeField] private DimensionUI dimensionUI;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameLabel;
    [SerializeField] private TMP_Text descLabel;
    [SerializeField] private TMP_Text typeLabel;
    [SerializeField] private LeadingZeroTextUI amountLabel;
    [SerializeField] private MyButton useButton;
    [SerializeField] private Image buttonImage;

    private int rewardId;
    private bool canUse;

    void Awake()
    {
        if (useButton != null) useButton.SetOnClick(OnUseClicked);
    }

    public void SetData(RewardBase reward, bool canUse)
    {
        SetDataAndShow(reward);
        useButton.gameObject.SetActiveIfChanged(canUse && reward.Type == RewardType.Use);
    }

    public void SetDataAndShow(RewardBase reward)
    {
        rewardId = reward.rewardId;
        icon.sprite = reward.IconSprite;
        nameLabel.text = reward.Name;
        descLabel.text = reward.Desc;
        int amount = Mgr.Save.RewardStockData.GetAmount(reward);
        amountLabel.SetValue(amount, reward.MaxStock);

        bool isUseType = reward.DisplayType == RewardDisplayType.Use;
        useButton.gameObject.SetActiveIfChanged(isUseType);
        if (isUseType)
        {
            int useMax = UseCase_Reward.GetUseMax(reward.rewardId);
            canUse = useMax > 0;
            buttonImage.color = canUse ? Color.deepSkyBlue : Color.gray2;
        }

        dimensionUI.SetDimension(reward.dimension);
    }

    private void OnUseClicked()
    {
        if (canUse)
        {
            var useUI = GetComponentInParent<OverlayStack>().rewardUseUI;
            useUI.SetData(rewardId);
            useUI.Show();
        }
        else
        {
            Mgr.Toast.Show(Mgr.Local.Get("ui-unavailable-item"));
        }
    }
}
