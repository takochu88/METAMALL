using System;
using System.Collections.Generic;
using UnityEngine;

public class UseCase_Reward : MonoBehaviour
{
    private Main main;
    private readonly List<RewardEntry> pending = new();

    public void Setup(Main main)
    {
        this.main = main;
    }

    /// <summary> チェックのみ（ストック変更なし） </summary>
    public StockCheckResult CheckReward(RewardBase reward, int amount)
    {
        return Mgr.Save.Stock.Check(reward.rewardId, amount, reward.MaxStock);
    }

    public StockCheckResult CheckReward(int rewardId, int amount)
    {
        if (!Mgr.Master.TryGetReward(rewardId, out RewardBase reward))
            return default;
        return CheckReward(reward, amount);
    }

    /// <summary> 確認なしで即Add </summary>
    public void AddReward(int rewardId, int amount, bool isForce = false)
    {
        if (!Mgr.Master.TryGetReward(rewardId, out RewardBase reward))
        {
            Debug.LogError($"[Reward] rewardId={rewardId} が見つかりません");
            return;
        }

        AddReward(reward, amount);
    }

    public void AddReward(RewardBase reward, int amount, bool isForce = false)
    {
        var stock = Mgr.Save.Stock;
        int rewardId = reward.rewardId;
        int max = reward.MaxStock;

        // 内部でもチェック（クランプ付きAdd）
        int actual = stock.Add(rewardId, amount, max);
        if (actual == 0) return;

        // Type毎の処理
        // switch (reward.Type) { }

        // GetRewardUIに登録
        pending.Add(new RewardEntry { rewardId = rewardId, amount = actual });
        
        switch (reward.Type)
        {
            case RewardType.UnlockCharacter:
                Mgr.Save.AllCharacterData.Unlock(reward.Id);
                break;
        }
    }

    /// <summary>
    /// チェック → オーバーフロー時は警告 → 確認後にAdd
    /// onConfirm: ユーザーが承認した場合のコールバック
    /// </summary>
    public void AddRewardWithCheck(RewardBase reward, int amount, Action onOverflowConfirm = null)
    {
        var result = CheckReward(reward, amount);

        if (result.IsFull)
        {
            // TODO: 「既にいっぱいです」通知
            return;
        }

        if (result.HasOverflow)
        {
            // TODO: 「{result.overflow}個オーバーします。あふれた分は削除されます。よろしいですか？」ダイアログ
            // ダイアログのOKコールバックで AddReward(reward, amount) を呼ぶ
            // 仮実装: onOverflowConfirm があればすぐ実行
            onOverflowConfirm?.Invoke();
            return;
        }

        AddReward(reward, amount);
    }

    /// <summary> 溜まったpendingをUIに表示 </summary>
    public void ShowRewards()
    {
        if (pending.Count == 0) return;

        var ui = main.ui.OverlayStack.GetOverlay<GetRewardUI>();
        ui.SetRewards(pending);
        ui.Show();
        pending.Clear();
    }
}
