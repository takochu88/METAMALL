using System;
using System.Collections.Generic;

/// <summary>
/// 受取済み / 既読 NewsId を管理するセーブデータ。
/// </summary>
[Serializable]
public class MailSaveData
{
    private readonly HashSet<string> receivedNewsIds = new();
    private readonly HashSet<string> readNewsIds = new();

    public bool IsReceived(string newsId) => receivedNewsIds.Contains(newsId);
    public void Receive(string newsId) => receivedNewsIds.Add(newsId);

    public bool IsRead(string newsId) => readNewsIds.Contains(newsId);
    public void MarkRead(string newsId) => readNewsIds.Add(newsId);

    /// <summary>
    /// バッジ対象件数を返す。
    /// 報酬あり → 未受取をカウント / 報酬なし → 未読をカウント。
    /// </summary>
    public int GetBadgeCount(IReadOnlyList<OneMailTitleOneData> mails)
    {
        int count = 0;
        for (int i = 0; i < mails.Count; i++)
        {
            var mail = mails[i];
            if (mail.HasRewards)
            {
                if (!IsReceived(mail.NewsId)) count++;
            }
            else
            {
                if (!IsRead(mail.NewsId)) count++;
            }
        }
        return count;
    }
}
