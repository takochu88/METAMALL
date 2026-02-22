using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アイテムとそのストック量を表示する汎用UI
/// </summary>
public class RewardAmountUI : MonoBehaviour
{
    [SerializeField] private Image rewardImage;
    [SerializeField] private MyButton button;
    [SerializeField] private RectTransform plusRect;
    [SerializeField] private AnimationTextUI animTextUI;
    [SerializeField] private int rewardId = -1;

    private RectTransform animTextRect;
    private float animTextBaseX;

    private void Awake()
    {
        button.SetOnClick(OnClick);
        animTextRect = animTextUI.GetComponent<RectTransform>();
        animTextBaseX = animTextRect.anchoredPosition.x;
    }

    private void Start()
    {
        //初期で指定があればデータをセットする（マスター初期化後に実行）
        if (rewardId > 0)
        {
            SetData(rewardId);
        }
    }
    
    public void SetData(int rewardId)
    {
        if (Mgr.Master.TryGetReward(rewardId, out var reward))
        {
            SetData(reward);
        }
    }

    public void SetData(RewardBase reward)
    {
        rewardId = reward.rewardId;
        rewardImage.sprite = reward.IconSprite;
        int amount = Mgr.Save.RewardStockData.GetAmount(reward);
        animTextUI.SetValue(amount, false);
        bool hasIncreaseMethod = reward.increaseMethodType != RewardIncreaseMethodType.None;
        plusRect.gameObject.SetActiveIfChanged(hasIncreaseMethod);
        float offsetX = hasIncreaseMethod ? -plusRect.sizeDelta.x : 0f;
        animTextUI.rect.anchoredPosition = new Vector2(animTextBaseX + offsetX, animTextUI.rect.anchoredPosition.y);
    }

    private void OnEnable()
    {
        Mgr.Save.RewardStockData.OnStockChanged += OnStockChanged;
    }

    private void OnDisable()
    {
        Mgr.Save.RewardStockData.OnStockChanged -= OnStockChanged;
    }

    private void OnStockChanged(int changedRewardId, int newAmount)
    {
        if (changedRewardId == rewardId)
            animTextUI.SetValue(newAmount);
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActiveIfChanged(visible);
    }

    private void OnClick()
    {
        UseCase_DisplayItem.OnShowRewardIncreaseMethods(rewardId);
    }
}
