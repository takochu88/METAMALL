using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MailUI : OverlayUIBase
{
    [SerializeField] private MyScrollRect scroll;

    [Header("詳細")]
    [SerializeField] private GameObject listPanel;
    [SerializeField] private GameObject detailPanel;
    [SerializeField] private TMP_Text bodyLabel;
    [SerializeField] private TMP_Text dateLabel;
    [SerializeField] private RewardListUI rewardListUI;
    [SerializeField] private MyButton claimButton;
    [SerializeField] private TMP_Text claimedLabel;

    private List<MailItem> mails;
    private Action<int> onMailSelected;
    private Action<MailItem> onClaim;
    private int selectedIndex = -1;

    public void Setup(List<MailItem> mails, Action<int> onMailSelected, Action<MailItem> onClaim)
    {
        this.mails = mails;
        this.onMailSelected = onMailSelected;
        this.onClaim = onClaim;
        if (claimButton != null) claimButton.SetOnClick(OnClaimClicked);
    }

    public void Refresh(List<MailItem> mails)
    {
        this.mails = mails;
        if (selectedIndex >= 0 && detailPanel != null && detailPanel.activeSelf)
            ShowDetail(selectedIndex);
        else
            InitScroll();
    }

    protected override void OnAfterShow()
    {
        Canvas.ForceUpdateCanvases();
        ShowList();
    }

    protected override void OnAfterHide()
    {
        selectedIndex = -1;
    }

    // ── 一覧 ──────────────────────────────
    private void ShowList()
    {
        if (listPanel != null) listPanel.SetActiveIfChanged(true);
        if (detailPanel != null) detailPanel.SetActiveIfChanged(false);
        selectedIndex = -1;
        InitScroll();
    }

    private void InitScroll()
    {
        if (mails == null || mails.Count == 0)
        {
            scroll.Init(0, null);
            return;
        }

        scroll.Init(mails.Count,
            cell =>
            {
                cell.Get<MailCellUI>().Init(OnRowClicked);
            },
            (index, cell) =>
            {
                var mail = mails[index];
                bool claimed = Mgr.Save.MailSaveData.IsClaimed(mail.NewsId);
                cell.Get<MailCellUI>().SetData(index, mail, claimed);
            });
    }

    private void OnRowClicked(int index)
    {
        onMailSelected?.Invoke(index);
    }

    // ── 詳細 ──────────────────────────────
    public void ShowDetail(int index)
    {
        if (index < 0 || index >= mails.Count) return;
        selectedIndex = index;
        var mail = mails[index];

        if (listPanel != null) listPanel.SetActiveIfChanged(false);
        if (detailPanel != null) detailPanel.SetActiveIfChanged(true);

        TitleText.text = mail.Title;
        bodyLabel.text = mail.Body;
        dateLabel.text = mail.Timestamp.ToString("yyyy/MM/dd HH:mm");

        bool claimed = Mgr.Save.MailSaveData.IsClaimed(mail.NewsId);
        bool canClaim = mail.HasRewards && !claimed && !mail.IsExpired;

        if (mail.HasRewards && rewardListUI != null)
        {
            rewardListUI.gameObject.SetActiveIfChanged(true);
            rewardListUI.Setup(mail.Rewards, RewardDisplayMode.AllAtOnce, RewardMergeMode.PreMerge);
        }
        else if (rewardListUI != null)
        {
            rewardListUI.gameObject.SetActiveIfChanged(false);
        }

        if (claimButton != null) claimButton.gameObject.SetActiveIfChanged(canClaim);
        if (claimedLabel != null) claimedLabel.gameObject.SetActiveIfChanged(claimed);
    }

    private void OnClaimClicked()
    {
        if (selectedIndex < 0 || selectedIndex >= mails.Count) return;
        onClaim?.Invoke(mails[selectedIndex]);
    }

    /// <summary> 戻るボタンで一覧に戻る </summary>
    public void BackToList()
    {
        ShowList();
    }
}
