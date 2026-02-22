using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UseCase_Reward : MonoBehaviour
{
    private static UseCase_Reward _instance;
    private Main main;
    private readonly List<RewardEntry> pending = new();
    private readonly List<RewardEntry> pendingRemoveds = new();

    public void Setup(Main main)
    {
        this.main = main;
        _instance = this;
        main.ui.OverlayStack.getRewardUI.Setup();
        main.ui.OverlayStack.rewardUseUI.Setup(OnConfirmUseReward);
    }

    /// <summary>使用可能な最大数を返す。現在はストック数。</summary>
    public static int GetUseMax(int rewardId)
    {
        if (!Mgr.Master.TryGetReward(rewardId, out var reward)) return 0;
        return Mgr.Save.RewardStockData.GetStock(reward.DisplayType).GetAmount(rewardId);
    }

    /// <summary>使用確認UIを表示し、OKなら消費する。</summary>
    public void OnConfirmUseReward(int rewardId, int amount)
    {
        if (!Mgr.Master.TryGetReward(rewardId, out var reward))
        {
            return;
        }
        
        var confirm = _instance.main.ui.OverlayStack.confirmUI;
        string title = Mgr.Local.Get("ui-use-item");
        string msg = Mgr.Local.Format("ui-confirm-use-item", reward.Name,  amount);
        confirm.SetData(title, msg, () => OnUseReward(rewardId, amount), null);
        confirm.Show();
    }

    /// <summary>リワードを消費する。</summary>
    public static void OnUseReward(int rewardId, int amount)
    {
        if (!Mgr.Master.TryGetReward(rewardId, out var reward)) return;
        var stock = Mgr.Save.RewardStockData.GetStock(reward.DisplayType);
        if (!stock.TryConsume(rewardId, amount)) return;
        Mgr.Save.RewardStockData.NotifyStockChanged(rewardId, stock.GetAmount(rewardId));

        var overlay = _instance.main.ui.OverlayStack;
        overlay.rewardDetailUI.Hide();
        overlay.rewardUseUI.Hide();
        _instance.main.useCase.inventory.OnRefresh();
        Mgr.Toast.Show(Mgr.Local.Get("ui-used-item"));
    }

    public void AddReward(int rewardId, int amount, bool isActive)
    {
        if (!Mgr.Master.TryGetReward(rewardId, out RewardBase reward)) return;
        
        AddReward(reward, amount, isActive);
    }

    public void AddReward(RewardBase reward, int amount, bool isActive)
    {
        RewardEntry entry = new RewardEntry(reward.rewardId, amount);
        AddReward(entry, isActive);
    }

    public void AddReward(RewardEntry entry, bool isActive)
    {
        List<RewardEntry> entries = new List<RewardEntry>();
        entries.Add(entry);
        AddRewardsAsync(entries, isActive);
    }
    
    public async UniTask AddRewardsAsync(List<RewardEntry> entries, bool isActive)
    {
        // 不正データチェック（1個でもNGなら即終了）
        if (!ValidateEntries(entries)) return;

        // 能動的な受け取り → オーバーフローチェック
        if (isActive)
        {
            List<RewardEntry> pendingOverflows = new();
            pendingOverflows.AddRange(GetOverflows(entries));
            
            if (pendingOverflows.Count > 0)
            {
                var rewardUI = main.ui.OverlayStack.getRewardUI;
                rewardUI.SetRewards(pendingOverflows, getRewardType: GetRewardType.OverFlow);
                await rewardUI.ShowAsync();

                if (!rewardUI.IsForceGet)
                {
                    return;
                }
            }
        }

        // 実際の加算処理（オーバーフロー分はクランプされる）
        ApplyRewards(entries);
    }
    
    void ApplyRewards(List<RewardEntry> entries)
    {
        bool hadNewUseItem = Mgr.Save.RewardStockData.HasNewUseItem;

        foreach (var entry in entries)
        {
            Mgr.Master.TryGetReward(entry.rewardId, out RewardBase reward);
            var stock = Mgr.Save.RewardStockData.GetStock(reward.DisplayType);
            int actual = stock.Add(entry.rewardId, entry.amount, reward.MaxStock);

            int removed = entry.amount - actual;
            if (removed > 0)
                pendingRemoveds.Add(new RewardEntry { rewardId = entry.rewardId, amount = removed });

            if (actual == 0) continue;

            pending.Add(new RewardEntry { rewardId = entry.rewardId, amount = actual });
            Mgr.Save.RewardStockData.NotifyStockChanged(entry.rewardId, stock.GetAmount(entry.rewardId));

            if (reward.DisplayType == RewardDisplayType.Use)
                Mgr.Save.RewardStockData.AddNewUseItem(entry.rewardId);

            switch (reward.Type)
            {
                case RewardType.UnlockCharacter:
                    Mgr.Save.AllCharacterData.Unlock(reward.Id);
                    break;
            }
        }

        if (!hadNewUseItem && Mgr.Save.RewardStockData.HasNewUseItem)
            main.useCase.inventory.OnRefresh();
    }


    /// <summary> 同じrewardIdを合算してからオーバーフローを判定。amount=あふれる量。 </summary>
    public List<RewardEntry> GetOverflows(List<RewardEntry> entries)
    {
        // rewardId ごとに合算
        var totals = new Dictionary<int, int>();
        foreach (var entry in entries)
        {
            if (totals.ContainsKey(entry.rewardId)) totals[entry.rewardId] += entry.amount;
            else totals[entry.rewardId] = entry.amount;
        }

        var result = new List<RewardEntry>();
        foreach (var kv in totals)
        {
            Mgr.Master.TryGetReward(kv.Key, out RewardBase reward);
            var stock = Mgr.Save.RewardStockData.GetStock(reward.DisplayType);
            var check = stock.Check(kv.Key, kv.Value, reward.MaxStock);
            if (check.HasOverflow)
            {
                result.Add(new RewardEntry { rewardId = kv.Key, amount = check.overflow });
            }
        }
        return result;
    }

    bool ValidateEntries(List<RewardEntry> entries)
    {
        for (int i = 0; i < entries.Count; i++)
        {
            var e = entries[i];
            if (e.amount <= 0)
            {
                Debug.LogError($"[Reward] entries[{i}] amount={e.amount} が不正です (rewardId={e.rewardId})");
                return false;
            }
            if (!Mgr.Master.TryGetReward(e.rewardId, out _))
                return false;
        }
        return true;
    }
    
    /// <summary> 溜まったpendingをUIに表示 </summary>
    public async void ShowRewards()
    {
        if (pending.Count >= 1)
        {
            var ui = main.ui.OverlayStack.GetOverlay<GetRewardUI>();
            ui.SetRewards(pending, GetRewardType.Get, display: RewardDisplayMode.AllAtOnce,
                merge: RewardMergeMode.PostMerge);
            pending.Clear();
            await ui.ShowAsync();
        }
        
        if (pendingRemoveds.Count > 0)
        {
            var ui = main.ui.OverlayStack.GetOverlay<GetRewardUI>();
            ui.SetRewards(pendingRemoveds, GetRewardType.Removed, display: RewardDisplayMode.AllAtOnce,
                merge: RewardMergeMode.PreMerge);
            pendingRemoveds.Clear();
            await ui.ShowAsync();
        }
    }
}
