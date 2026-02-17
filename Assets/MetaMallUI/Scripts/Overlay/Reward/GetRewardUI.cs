using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GetRewardUI : OverlayUIBase
{
    [SerializeField] Transform itemContainer;
    [FormerlySerializedAs("itemIconPrefab")] [SerializeField] RewardIconUI rewardIconPrefab;

    private readonly List<RewardIconUI> activeIcons = new();
    private List<RewardEntry> rewards;

    public void SetRewards(List<RewardEntry> rewards)
    {
        this.rewards = new List<RewardEntry>(rewards);
    }

    protected override void OnAfterShow()
    {
        ClearIcons();
        foreach (var reward in rewards)
        {
            var icon = Instantiate(rewardIconPrefab, itemContainer);
            icon.SetData(reward.rewardId, reward.amount);
            activeIcons.Add(icon);
        }
    }

    protected override void OnAfterHide()
    {
        ClearIcons();
    }

    private void ClearIcons()
    {
        foreach (var icon in activeIcons)
            Destroy(icon.gameObject);
        activeIcons.Clear();
    }
}
