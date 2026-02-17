using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ContainerRowUI : MonoBehaviour
{
    [FormerlySerializedAs("itemIcon")] [SerializeField] RewardIconUI rewardIcon;
    [SerializeField] TMP_Text descLabel;

    public void SetData(RewardBase reward)
    {
       // itemIcon.SetData(reward.rewardId, reward.amount);
    }
}
