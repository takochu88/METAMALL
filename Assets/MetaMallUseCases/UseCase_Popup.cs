using UnityEngine;

public class UseCase_Popup : MonoBehaviour
{
    Main main;

    public void Setup(Main main)
    {
        this.main = main;
    }

    public void OnUpdate()
    {
        if (!main.ui.IsInteractable) return;

        //獲得したアイテムを表示する
        if (!main.ui.OverlayStack.getRewardUI.IsShown)
        {
            main.useCase.reward.ShowRewards();
        }
        
    }
}
