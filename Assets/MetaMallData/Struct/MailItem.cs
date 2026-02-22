using System;
using System.Collections.Generic;

/// <summary>
/// パース済みメール1通分のデータ。
/// </summary>
public class MailItem
{
    public string NewsId;
    public string Title;
    public string Body;
    public DateTime Timestamp;
    public DateTime? ExpireAt;
    public List<RewardEntry> Rewards;

    public bool HasRewards => Rewards != null && Rewards.Count > 0;
    public bool IsExpired => ExpireAt.HasValue && DateTime.UtcNow >= ExpireAt.Value;
}
