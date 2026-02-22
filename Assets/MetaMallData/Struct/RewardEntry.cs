/// <summary> 報酬ID＋個数のペア（UI表示・受け渡し用） </summary>
[System.Serializable]
public struct RewardEntry
{
    public int rewardId;
    public int amount;

    public RewardEntry(int rewardId, int amount)
    {
        this.rewardId = rewardId;
        this.amount = amount;
    }
}
