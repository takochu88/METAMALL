using System;
using System.Collections.Generic;

/// <summary>
/// TitleNews Body JSON のデシリアライズ用。
/// </summary>
[Serializable]
public class MailBodyData
{
    public string body;
    public List<RewardEntry> rewards;
    public string expireAt;
}
