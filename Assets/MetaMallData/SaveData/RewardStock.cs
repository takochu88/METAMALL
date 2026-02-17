using System;
using System.Collections.Generic;

public struct StockCheckResult
{
    public int current;   // 現在の所持数
    public int addable;   // 実際に加算できる量
    public int overflow;  // あふれる量（削除される量）
    public int max;       // 上限値

    public bool CanAddAll => overflow == 0;
    public bool IsFull => addable == 0;
    public bool HasOverflow => overflow > 0;
}

[Serializable]
public class RewardStock
{
    private readonly Dictionary<int, int> stock = new();

    public int GetAmount(int rewardId)
    {
        return stock.TryGetValue(rewardId, out var amount) ? amount : 0;
    }

    /// <summary>
    /// 加算前のチェック。ストックは変更しない。
    /// </summary>
    public StockCheckResult Check(int rewardId, int amount, int max)
    {
        int current = GetAmount(rewardId);
        int clamped = Math.Clamp(current + amount, 0, max);
        int addable = clamped - current;

        return new StockCheckResult
        {
            current = current,
            addable = addable,
            overflow = amount - addable,
            max = max,
        };
    }

    /// <summary>
    /// ストックに加算する。実際に加算された量を返す。
    /// 内部でもクランプチェックを行う。
    /// </summary>
    public int Add(int rewardId, int amount, int max)
    {
        var result = Check(rewardId, amount, max);
        if (result.addable != 0)
            stock[rewardId] = result.current + result.addable;
        return result.addable;
    }

    public void Set(int rewardId, int amount, int max)
    {
        stock[rewardId] = Math.Clamp(amount, 0, max);
    }

    public bool HasStock(int rewardId)
    {
        return GetAmount(rewardId) > 0;
    }

    public bool ShouldDisplay(RewardBase reward)
    {
        return reward.showWhenEmpty || HasStock(reward.rewardId);
    }
}
