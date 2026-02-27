using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// パース済みメールデータの一覧を保持する。
/// </summary>
public class MailTitleData
{
    private readonly List<OneMailTitleOneData> parsed = new();
    private readonly List<OneMailTitleOneData> published = new();

    /// <summary> パース済み全件（期限切れ含む） </summary>
    public IReadOnlyList<OneMailTitleOneData> Parsed => parsed;

    /// <summary> バリデート済み公開メール（期限切れ除外） </summary>
    public IReadOnlyList<OneMailTitleOneData> PublishedMails => published;
    public int Count => published.Count;

    public void Clear()
    {
        parsed.Clear();
        published.Clear();
    }

    /// <summary> TitleNews 一覧からメールをパースし、公開用リストを生成する。 </summary>
    public void Parse(IReadOnlyList<PlayFab.ClientModels.TitleNewsItem> newsList)
    {
        parsed.Clear();
        foreach (var news in newsList)
        {
            var mail = ParseOne(news);
            if (mail != null) parsed.Add(mail);
        }

        Publish();
    }

    /// <summary> パース済みデータからバリデートして公開用リストを更新する。 </summary>
    public void Publish()
    {
        published.Clear();
        for (int i = 0; i < parsed.Count; i++)
        {
            if (!parsed[i].IsExpired) published.Add(parsed[i]);
        }
        published.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));
    }

    private static OneMailTitleOneData ParseOne(PlayFab.ClientModels.TitleNewsItem news)
    {
        if (string.IsNullOrEmpty(news.Body)) return null;

        try
        {
            var data = JsonUtility.FromJson<OneMailTitleOneData>(news.Body);
            data.NewsId = news.NewsId;
            data.Title = news.Title;
            data.Timestamp = news.Timestamp;
            ValidateRewards(data);
            return data;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Mail] Body パース失敗 (NewsId={news.NewsId}): {e.Message}");
            return null;
        }
    }

    /// <summary> 存在しない rewardId を除外する。 </summary>
    private static void ValidateRewards(OneMailTitleOneData data)
    {
        if (data.rewards == null) return;
        for (int i = data.rewards.Count - 1; i >= 0; i--)
        {
            if (!Mgr.Master.TryGetReward(data.rewards[i].rewardId, out _))
            {
                Debug.LogWarning($"[Mail] 不明な rewardId={data.rewards[i].rewardId} を除外 (NewsId={data.NewsId})");
                data.rewards.RemoveAt(i);
            }
        }
    }
}
