using System.Collections.Generic;
using UnityEngine;

public class UseCase_Mail : UseCase_MenuBase
{
    protected override MenuType MenuType => MenuType.Mail;

    private MailUI mailUI;
    private readonly MailTitleData mailData = new();

    public MailTitleData MailData => mailData;

    public void Setup(Main main)
    {
        this.main = main;
        mailUI = main.ui.OverlayStack.mailUI;
        main.ui.upperMenuUI.selectMailUI.SetupSub(OnShowMail);
        mailUI.Setup(OnSelectCell, OnReceiveClicked, OnMailHide);
        main.useCase.titleData.AddListener(OnFetchTitleData);
    }
    
    private void OnFetchTitleData(IReadOnlyList<PlayFab.ClientModels.TitleNewsItem> newsList)
    {
        mailData.Parse(newsList);
        OnRefresh();
        Debug.Log($"[Mail] パース完了: {mailData.Count} 件");
    }
    
    public void OnShowMail()
    {
        if (!main.useCase.titleData.IsFetched) return;
        mailData.Publish();
        mailUI.Refresh(mailData.PublishedMails);
        mailUI.Show();
        if (mailData.Count > 0) OnSelectCell(SelectedIndex);
    }

    protected override void OnSelectCell(int index)
    {
        if (index < 0 || index >= mailData.Count) return;
        base.OnSelectCell(index);
        var mail = mailData.PublishedMails[index];

        // 報酬なしメールは選択時に既読にする
        if (!mail.HasRewards)
            Mgr.Save.MailSaveData.MarkRead(mail.NewsId);

        bool received = Mgr.Save.MailSaveData.IsReceived(mail.NewsId);
        mailUI.RefreshCell(index);
        mailUI.ShowDetail(mail, received, index);
        UpdateBadge();
    }

    private void OnReceiveClicked()
    {
        if (SelectedIndex < 0 || SelectedIndex >= mailData.Count) return;
        OnReceiveReward(mailData.PublishedMails[SelectedIndex]);
    }

    private async void OnReceiveReward(OneMailTitleOneData mail)
    {
        if (mail == null || !mail.HasRewards) return;
       
        if (Mgr.Save.MailSaveData.IsReceived(mail.NewsId))
        {
            Mgr.Toast.Show(Mgr.Local.Get("ui-already-received"));
            return;
        }
        
        await main.useCase.reward.AddRewardsAsync(mail.rewards, isActive: true, rejectOverflow: true);
        
        Mgr.Save.MailSaveData.Receive(mail.NewsId);
        main.useCase.reward.ShowRewards();
        mailUI.Refresh(mailData.PublishedMails);
        OnSelectCell(SelectedIndex);
        UpdateBadge();
    }
    
    public void OnRefresh()
    {
        UpdateBadge();
    }
    
    public void UpdateBadge()
    {
        int count = Mgr.Save.MailSaveData.GetBadgeCount(mailData.PublishedMails);
        main.ui.upperMenuUI.selectMailUI.badge.SetVisible(count > 0);
    }
    
    private void OnMailHide()
    {
        UpdateBadge();
    }
}
