using System;
using System.Collections.Generic;

[Serializable]
public class RewardStockData
{
    private readonly Dictionary<RewardDisplayType, Stock> stocks = new()
    {
        { RewardDisplayType.Currency, new Stock() },
        { RewardDisplayType.Ticket,   new Stock() },
        { RewardDisplayType.Other,    new Stock() },
        { RewardDisplayType.Use,      new Stock() },
    };

    /// <summary> ストック変動時に発火（rewardId, newAmount） </summary>
    public event Action<int, int> OnStockChanged;

    private readonly HashSet<int> newUseItemIds = new();

    /// <summary> newUseItemIds が変化したときに発火 </summary>
    public event Action OnNewUseItemsChanged;

    public bool HasNewUseItem => newUseItemIds.Count > 0;

    public void AddNewUseItem(int rewardId)
    {
        if (newUseItemIds.Add(rewardId))
            OnNewUseItemsChanged?.Invoke();
    }

    public bool IsNewUseItem(int rewardId) => newUseItemIds.Contains(rewardId);

    public bool RemoveNewUseItem(int rewardId)
    {
        bool removed = newUseItemIds.Remove(rewardId);
        if (removed) OnNewUseItemsChanged?.Invoke();
        return removed;
    }

    public void ClearNewUseItems()
    {
        if (newUseItemIds.Count > 0)
        {
            newUseItemIds.Clear();
            OnNewUseItemsChanged?.Invoke();
        }
    }

    public Stock GetStock(RewardDisplayType type) => stocks[type];

    /// <summary> RewardBase から直接所持数を取得 </summary>
    public int GetAmount(RewardBase reward) => GetStock(reward.DisplayType).GetAmount(reward.rewardId);

    /// <summary> ストック変動を通知する </summary>
    public void NotifyStockChanged(int rewardId, int newAmount) => OnStockChanged?.Invoke(rewardId, newAmount);

    [Serializable]
    public class Stock
    {
        private readonly Dictionary<int, int> data = new();

        public int GetAmount(int rewardId)
        {
            return data.TryGetValue(rewardId, out var amount) ? amount : 0;
        }

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

        public int Add(int rewardId, int amount, int max)
        {
            var result = Check(rewardId, amount, max);
            if (result.addable != 0)
                data[rewardId] = result.current + result.addable;
            return result.addable;
        }

        public void Set(int rewardId, int amount, int max)
        {
            data[rewardId] = Math.Clamp(amount, 0, max);
        }

        /// <summary> 消費を試みる。足りなければ false を返し何もしない。 </summary>
        public bool TryConsume(int rewardId, int amount)
        {
            if (amount <= 0 || GetAmount(rewardId) < amount) return false;
            data[rewardId] -= amount;
            return true;
        }

        /// <summary> 消費可能かチェックのみ </summary>
        public bool CanConsume(int rewardId, int amount)
        {
            return amount > 0 && GetAmount(rewardId) >= amount;
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

    public struct StockCheckResult
    {
        public int current;
        public int addable;
        public int overflow;
        public int max;

        public bool CanAddAll => overflow == 0;
        public bool IsFull => addable == 0;
        public bool HasOverflow => overflow > 0;
    }
}
