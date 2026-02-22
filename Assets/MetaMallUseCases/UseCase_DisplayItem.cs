using UnityEngine;

public class UseCase_DisplayItem : MonoBehaviour
{
    private static UseCase_DisplayItem _instance;
    private Main main;

    public void Setup(Main main)
    {
        this.main = main;
        _instance = this;
    }

    /// <summary> static でどこからでもアイテム詳細UIを表示 </summary>
    public static void OnShowRewardDetail(RewardBase reward, bool canUse)
    {
        var ui = _instance.main.ui.OverlayStack.GetOverlay<RewardDetailUI>();
        ui.SetData(reward, canUse);
        ui.Show();
    }

    /// <summary> static でどこからでもアイテムを増やす手段を表示 </summary>
    public static void OnShowRewardIncreaseMethods(int rewardId)
    {
        if (Mgr.Master.TryGetReward(rewardId, out var reward))
        {
            OnShowRewardIncreaseMethods(reward);
        }
        
        //TODO 未実装
    }

    public static void OnShowRewardIncreaseMethods(RewardBase reward)
    {
        RewardIncreaseMethodType increaseMethodType = reward.increaseMethodType;
        
        if (increaseMethodType == RewardIncreaseMethodType.None)
        {
            OnShowRewardDetail(reward, canUse: false);
            return;
        }

        switch (increaseMethodType)
        {
            //TODO 未実装
        }
    }
}
