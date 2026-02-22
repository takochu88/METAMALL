using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UseCase_Mail : MonoBehaviour
{
    private Main main;
    private readonly List<MailItem> mails = new();

    public IReadOnlyList<MailItem> Mails => mails;

    public void Setup(Main main)
    {
        this.main = main;
        main.ui.upperMenuUI.selectMailUI.SetupSub(ShowMail);
        main.ui.OverlayStack.mailUI.Setup(mails, OnMailSelected, OnClaimReward);
    }

    /// <summary> PlayFab から TitleNews を取得してメール一覧を更新する </summary>
    public void FetchMails(Action onComplete = null)
    {
        if (!Mgr.PlayFab.IsLoggedIn)
        {
            Debug.LogWarning("[Mail] PlayFab 未ログイン");
            onComplete?.Invoke();
            return;
        }

        Mgr.PlayFab.GetTitleNews(
            newsList =>
            {
                mails.Clear();
                foreach (var news in newsList)
                {
                    var mail = ParseNewsItem(news);
                    if (mail != null && !mail.IsExpired) mails.Add(mail);
                }

                // 新しい順にソート
                mails.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));

                Debug.Log($"[Mail] パース完了: {mails.Count} 件");
                onComplete?.Invoke();
            },
            error =>
            {
                Debug.LogError($"[Mail] 取得失敗: {error}");
                onComplete?.Invoke();
            });
    }

    /// <summary> メール一覧を表示する（自動で最新データを取得） </summary>
    public void ShowMail()
    {
        Mgr.Loading.Show();
        FetchMails(() =>
        {
            Mgr.Loading.Hide();
            main.ui.OverlayStack.mailUI.Refresh(mails);
            main.ui.OverlayStack.mailUI.Show();
        });
    }

    private void OnMailSelected(int index)
    {
        if (index < 0 || index >= mails.Count) return;
        main.ui.OverlayStack.mailUI.ShowDetail(index);
    }

    private async void OnClaimReward(MailItem mail)
    {
        if (mail == null || !mail.HasRewards) return;
        if (Mgr.Save.MailSaveData.IsClaimed(mail.NewsId)) return;

        // 報酬を付与
        await main.useCase.reward.AddRewardsAsync(mail.Rewards, isActive: true);

        // 受取済みに記録
        Mgr.Save.MailSaveData.Claim(mail.NewsId);

        // 報酬取得演出
        main.useCase.reward.ShowRewards();

        // メール一覧を更新
        main.ui.OverlayStack.mailUI.Refresh(mails);
    }

    /// <summary> 未受取件数を取得する </summary>
    public int GetUnclaimedCount()
    {
        return Mgr.Save.MailSaveData.UnclaimedCount(mails);
    }

    private static MailItem ParseNewsItem(PlayFab.ClientModels.TitleNewsItem news)
    {
        var mail = new MailItem
        {
            NewsId = news.NewsId,
            Title = news.Title,
            Timestamp = news.Timestamp,
        };

        if (string.IsNullOrEmpty(news.Body))
        {
            mail.Body = "";
            mail.Rewards = null;
            return mail;
        }

        try
        {
            var bodyData = JsonUtility.FromJson<MailBodyData>(news.Body);
            mail.Body = bodyData.body ?? "";
            mail.Rewards = bodyData.rewards;
            if (!string.IsNullOrEmpty(bodyData.expireAt) &&
                DateTime.TryParse(bodyData.expireAt, null, System.Globalization.DateTimeStyles.RoundtripKind, out var expire))
            {
                mail.ExpireAt = expire;
            }
        }
        catch (Exception e)
        {
            // JSON でない場合はそのまま本文として使う
            Debug.LogWarning($"[Mail] Body パース失敗 (NewsId={news.NewsId}): {e.Message}");
            mail.Body = news.Body;
            mail.Rewards = null;
        }

        return mail;
    }
}
