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
    public static void OnShowItemDetail(int rewardId)
    {
        var ui = _instance.main.ui.OverlayStack.GetOverlay<RewardDetailUI>();
        ui.SetData(rewardId);
        ui.Show();
    }
}
