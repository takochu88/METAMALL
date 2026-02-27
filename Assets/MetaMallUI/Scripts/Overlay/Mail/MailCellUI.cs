using TMPro;
using UnityEngine;

public class MailCellUI : CellUIBase
{
    [SerializeField] private TMP_Text remainingTimeText;
    [SerializeField] private RewardIconUI rewardIconUI;
    [SerializeField] private TMP_Text moreText;

    public void SetData(OneMailTitleOneData mail, bool received, bool read)
    {
        Title.text = mail.Title;
        remainingTimeText.text = MetaMallUtils.ToRemainingTimeString(mail.ExpireAtUtc) ?? "";
        bool showBadge = mail.HasRewards ? !received : !read;
        SetBadgeVisible(showBadge);

        if (mail.HasRewards)
        {
            rewardIconUI.SetVisible(true);
            rewardIconUI.SetData(mail.rewards[0].rewardId, RewardIconDisplayType.Amount, mail.rewards[0].amount);
            rewardIconUI.SetVisibleDark(received);
            moreText.enabled = mail.rewards.Count > 1;
        }
        else
        {
            rewardIconUI.SetVisible(false);
            moreText.enabled = false;
        }
    }
}
