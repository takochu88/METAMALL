using System;
using System.Collections.Generic;

/// <summary>
/// 受取済み NewsId を管理するセーブデータ。
/// </summary>
[Serializable]
public class MailSaveData
{
    private readonly HashSet<string> claimedNewsIds = new();

    public bool IsClaimed(string newsId) => claimedNewsIds.Contains(newsId);
    public void Claim(string newsId) => claimedNewsIds.Add(newsId);
    public int UnclaimedCount(IReadOnlyList<MailItem> mails)
    {
        int count = 0;
        for (int i = 0; i < mails.Count; i++)
        {
            if (mails[i].HasRewards && !IsClaimed(mails[i].NewsId))
                count++;
        }
        return count;
    }
}
