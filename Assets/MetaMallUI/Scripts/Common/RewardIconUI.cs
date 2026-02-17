using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardIconUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text amountLabel;
    [SerializeField] MyButton button;

    private int rewardId;

    private void Awake()
    {
        button.SetOnClick(OnClick);
    }
    
    public void SetData(int rewardId, int amount = -1)
    {
        this.rewardId = rewardId;
        bool showAmount = amount >= 2; // 2以上のときだけ "xN" を出す
        amountLabel.text = showAmount ? $"x{amount}" : "";
        amountLabel.gameObject.SetActiveIfChanged(showAmount);
    }
    
    private void OnClick()
    {
        UseCase_DisplayItem.OnShowItemDetail(rewardId);
    }
}
