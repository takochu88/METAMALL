using System.Collections.Generic;
using System.Linq;

public static partial class DebugCommandList
{
   /* static void RegisterRewardCommands()
    {
        Add("リワードテスト: AllAtOnce", DebugCategory.Reward, main =>
        {
            var list = BuildTestRewards(main, itemCount: 5, amountEach: 100);
            var ui = main.ui.OverlayStack.GetOverlay<GetRewardUI>();
            ui.SetRewards(list, RewardDisplayMode.AllAtOnce, RewardMergeMode.NoMerge, iconSize: 140);
            ui.Show();
        });

        Add("リワードテスト: OneByOne", DebugCategory.Reward, main =>
        {
            var list = BuildTestRewards(main, itemCount: 5, amountEach: 100);
            var ui = main.ui.OverlayStack.GetOverlay<GetRewardUI>();
            ui.SetRewards(list, RewardDisplayMode.OneByOne, RewardMergeMode.NoMerge, iconSize: 140);
            ui.Show();
        });

        Add("リワードテスト: PreMerge", DebugCategory.Reward, main =>
        {
            // 同じリワードを複数回に分けて追加 → マージして表示
            var ids = GetValidRewardIds(main, 3);
            var list = new List<RewardEntry>();
            foreach (var id in ids)
                for (int i = 0; i < 4; i++)
                    list.Add(new RewardEntry { rewardId = id, amount = 100 });
            var ui = main.ui.OverlayStack.GetOverlay<GetRewardUI>();
            ui.SetRewards(list, RewardDisplayMode.AllAtOnce, RewardMergeMode.PreMerge, iconSize: 140);
            ui.Show();
        });

        Add("リワードテスト: PostMerge", DebugCategory.Reward, main =>
        {
            // 同じリワードを複数回に分けて追加 → 1個ずつ表示後にマージ
            var ids = GetValidRewardIds(main, 3);
            var list = new List<RewardEntry>();
            foreach (var id in ids)
                for (int i = 0; i < 4; i++)
                    list.Add(new RewardEntry { rewardId = id, amount = 100 });
            var ui = main.ui.OverlayStack.GetOverlay<GetRewardUI>();
            ui.SetRewards(list, RewardDisplayMode.OneByOne, RewardMergeMode.PostMerge, iconSize: 140);
            ui.Show();
        });

        Add("リワードテスト: 大量 (20種)", DebugCategory.Reward, main =>
        {
            var list = BuildTestRewards(main, itemCount: 20, amountEach: 100);
            var ui = main.ui.OverlayStack.GetOverlay<GetRewardUI>();
            ui.SetRewards(list, RewardDisplayMode.OneByOne, RewardMergeMode.NoMerge, iconSize: 140);
            ui.Show();
        });
    }
    
    static List<int> GetValidRewardIds(Main main, int count)
    {
        return Mgr.Master.AllRewardMasters.Keys
            .OrderBy(id => id)
            .Take(count)
            .ToList();
    }*/
}
