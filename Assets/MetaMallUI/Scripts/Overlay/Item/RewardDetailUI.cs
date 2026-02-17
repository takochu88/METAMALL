using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardDetailUI : OverlayUIBase
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text nameLabel;
    [SerializeField] TMP_Text descLabel;
    [SerializeField] TMP_Text typeLabel;
    [SerializeField] TMP_Text amountLabel;
    
    private int rewardId;

    public void SetData(int rewardId)
    {
        if(!Mgr.Master.TryGetReward(rewardId, out var reward))
        {
            Debug.LogError($"[Reward] rewardId={rewardId} が見つかりません");
            return;
        }

        SetDataAnd(reward);
    }

    public void SetDataAnd(RewardBase reward)
    {
        nameLabel.text = reward.Name;
        descLabel.text = reward.Desc;
        amountLabel.text = Mgr.Save.Stock.GetAmount(reward.rewardId).ToString();
    }

    private MasterBase GetMaster(RewardType type, int itemId)
    {
        return type switch
        {
            RewardType.UnlockCharacter => Mgr.Master.characterTable[itemId],
            RewardType.Parallel        => Mgr.Master.parallelTable[itemId],
            RewardType.Equipment       => Mgr.Master.equipmentTable[itemId],
            RewardType.Ability         => Mgr.Master.abilityTable[itemId],
            _ => null,
        };
    }

    private string GetTypeName(RewardType type)
    {
        return type switch
        {
            RewardType.UnlockCharacter => "キャラクター",
            RewardType.Parallel        => "パラレル",
            RewardType.Equipment       => "装備",
            RewardType.Ability         => "アビリティ",
            _ => "その他",
        };
    }
}
