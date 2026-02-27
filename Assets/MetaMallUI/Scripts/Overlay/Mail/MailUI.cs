using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MailUI : OverlayUIBase
{
    [Header("詳細")]
    [SerializeField] private TMP_Text mailTitleLabel;
    [SerializeField] private TMP_Text bodyLabel;
    [SerializeField] private TMP_Text dateLabel;
    [SerializeField] private RewardListUI rewardListUI;
    [SerializeField] private MyButton receiveButton;
    [SerializeField] private TMP_Text receiveLabel;
    [SerializeField] private Image receiveButtonImage;
    [SerializeField] private ScrollRect bodyScrollRect;
    [SerializeField] private MyScrollRect scroll;
    [SerializeField] private TMP_Text mailCountText;
    [SerializeField] private float bodyBottomNoRewards;

    private IReadOnlyList<OneMailTitleOneData> mails;
    private Action onHide;
    private float bodyBottomDefault;

    public void Setup(Action<int> onSelectCell, Action onReceive, Action onHide = null)
    {
        this.onHide = onHide;
        scroll.InitSelectCell(onSelectCell);
        if (receiveButton != null) receiveButton.SetOnClick(() => onReceive?.Invoke());
        if (bodyScrollRect != null)
            bodyBottomDefault = ((RectTransform)bodyScrollRect.transform).offsetMin.y;
    }

    public void Refresh(IReadOnlyList<OneMailTitleOneData> mails)
    {
        this.mails = mails;
        RefreshScroll();
    }

    public void ShowList()
    {
        RefreshScroll();
        mailCountText.text = Mgr.Local.Format("ui-mail-count", mails.Count);
    }

    public void ShowDetail(OneMailTitleOneData mail, bool received, int index)
    {
        mailTitleLabel.text = mail.Title;
        bodyLabel.text = mail.body;
        if (bodyScrollRect != null)
        {
            var rt = (RectTransform)bodyScrollRect.transform;
            float bottom = mail.HasRewards ? bodyBottomDefault : bodyBottomNoRewards;
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);

            bodyScrollRect.velocity = Vector2.zero;
            bodyScrollRect.verticalNormalizedPosition = 1f;
            LayoutRebuilder.ForceRebuildLayoutImmediate(bodyScrollRect.content);
            bool needScroll = bodyScrollRect.content.rect.height > bodyScrollRect.viewport.rect.height;
            bodyScrollRect.vertical = needScroll;
            if (bodyScrollRect.verticalScrollbar != null)
                bodyScrollRect.verticalScrollbar.gameObject.SetActiveIfChanged(needScroll);
        }
        dateLabel.text = mail.Timestamp.ToString("yyyy/MM/dd HH:mm");

        if (mail.HasRewards && rewardListUI != null)
        {
            rewardListUI.gameObject.SetActiveIfChanged(true);
            rewardListUI.Setup(mail.rewards, RewardDisplayMode.AllAtOnce, RewardMergeMode.PreMerge, animated: false);
            receiveButton.gameObject.SetActiveIfChanged(true);
        }
        else if (rewardListUI != null)
        {
            rewardListUI.gameObject.SetActiveIfChanged(false);
            receiveButton.gameObject.SetActiveIfChanged(false);
        }

        UpdateReward(mail, received);
        scroll.UpdateSelection(index);
    }

    public void RefreshCell(int index)
    {
        var cell = scroll.FindVisibleCell(index);
        if (cell != null) OnUpdateCell(index, cell);
    }

    public void UpdateReward(OneMailTitleOneData mail, bool received)
    {
        if (mail.HasRewards && rewardListUI != null)
        {
            rewardListUI.gameObject.SetActiveIfChanged(true);
            rewardListUI.Setup(mail.rewards, RewardDisplayMode.AllAtOnce, RewardMergeMode.PreMerge, animated: false, dark: received);
            receiveButton.gameObject.SetActiveIfChanged(true);
        }
        else if (rewardListUI != null)
        {
            rewardListUI.gameObject.SetActiveIfChanged(false);
            receiveButton.gameObject.SetActiveIfChanged(false);
        }
        
        receiveButtonImage.color = received ? Color.gray2 : Color.deepSkyBlue;
        receiveLabel.text = received ? Mgr.Local.Get("ui-received") : Mgr.Local.Get("ui-receive");
    }

    /// <summary> 戻るボタンで一覧に戻る </summary>
    public void BackToList()
    {
        ShowList();
    }

    protected override void OnAfterShow()
    {
        base.OnAfterShow();
        Canvas.ForceUpdateCanvases();
        ShowList();
    }

    private void RefreshScroll()
    {
        if (mails == null || mails.Count == 0)
        {
            scroll.Init(0, null);
            return;
        }

        scroll.Init(mails.Count, OnUpdateCell);
        scroll.UpdateSelection(Mgr.Save.SessionData.GetIndex(MenuType.Mail));
    }

    private void OnUpdateCell(int index, MyScrollCell cell)
    {
        var row = (MailCellUI)cell;
        var mail = mails[index];
        bool received = Mgr.Save.MailSaveData.IsReceived(mail.NewsId);
        bool read = Mgr.Save.MailSaveData.IsRead(mail.NewsId);
        row.SetData(mail, received, read);
    }

    protected override void OnAfterHide()
    {
        base.OnAfterHide();
        onHide?.Invoke();
    }
}
