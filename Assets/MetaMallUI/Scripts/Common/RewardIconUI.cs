using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class RewardIconUI : MonoBehaviour
{
    [SerializeField] private LeadingZeroTextUI amountText;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Image icon;
    [SerializeField] private MyButton button;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private BadgeUI badgeUI;

    private int rewardId;
    private float baseSize;
    private float baseFontSize;
    private RewardIconDisplayType displayType;
    public bool CanUse => displayType == RewardIconDisplayType.StockAmount;
    
    private void Awake()
    {
        baseSize = rect.sizeDelta.x;
        baseFontSize = amountText.GetFontSize();
        button.SetOnClick(OnClick);
    }

    public void SetData(int rewardId, RewardIconDisplayType displayType, int amount = 1)
    {
        if (!Mgr.Master.TryGetReward(rewardId, out var reward))
        {
            return;
        }
        
        SetData(reward, displayType, amount);
    }

    public void SetData(RewardBase reward, RewardIconDisplayType displayType, int amount = 1)
    {
        this.rewardId = reward.rewardId;
        this.displayType = displayType;
        icon.sprite = reward.IconSprite;

        switch (displayType)
        {
            case RewardIconDisplayType.Default :
                amountText.SetiVisible(false);
                break;
            
            case RewardIconDisplayType.Amount :
                amountText.SetiVisible(true);
                amountText.SetValue(amount);
                break;
            
            case RewardIconDisplayType.StockAmount:
                amountText.SetiVisible(true);
                amountText.SetValue(amount, reward.MaxStock);
                break;
        }

        badgeUI.SetVisible(Mgr.Save.RewardStockData.IsNewUseItem(rewardId)　&& CanUse);
    }

    public void SetSize(float size)
    {
        rect.sizeDelta = new Vector2(size, size);
        if (baseSize > 0f)
            amountText.SetFontSize(baseFontSize * (size / baseSize));
    }

    /// <summary>出現前状態にリセット（scale=0, alpha=0, active=true）。</summary>
    public void PrepareReveal()
    {
        SetVisible(true);
        transform.localScale = Vector3.zero;
        canvasGroup.alpha = 0f;
    }

    /// <summary>出現アニメーション → Sequence を返す。</summary>
    public Sequence PlayReveal(float duration, Ease ease)
    {
        return Sequence.Create()
            .Group(Tween.Scale(transform, Vector3.one, duration, ease))
            .Group(Tween.Alpha(canvasGroup, 1f, duration * 0.5f));
    }

    /// <summary>即座に表示状態にする（リサイクル時用）。</summary>
    public void ShowImmediate()
    {
        transform.localScale = Vector3.one;
        if (canvasGroup != null) canvasGroup.alpha = 1f;
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActiveIfChanged(visible);
    }
    
    private void OnClick()
    {
        if(!Mgr.Master.TryGetReward(rewardId, out var reward)) return;

        if (CanUse && Mgr.Save.RewardStockData.RemoveNewUseItem(rewardId) && badgeUI != null)
            badgeUI.SetVisible(false);

        UseCase_DisplayItem.OnShowRewardDetail(reward, CanUse);
    }
}
