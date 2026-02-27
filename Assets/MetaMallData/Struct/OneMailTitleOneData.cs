using System;
using System.Collections.Generic;

/// <summary>
/// TitleNews Body JSON のデシリアライズ用（メール1通分）。
/// </summary>
[Serializable]
public class OneMailTitleOneData : ServerTitleOneDataBase
{
    public List<RewardEntry> rewards;

    public bool HasRewards => rewards != null && rewards.Count > 0;
}
