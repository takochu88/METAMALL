using UnityEngine;

[DefaultExecutionOrder(0)]
public class Main : MonoBehaviour
{
    public UseCases useCase;
    public MetaMallUI ui;
  
    void  Awake()
    {
        Mgr.Instance.Setup();
        useCase.Setup(this);
        ui.Setup(this);
        Test();
        useCase.topMenu.OnRefresh();

        Mgr.PlayFab.Login();
    }

    private void Update()
    {
        useCase.popup.OnUpdate();
        useCase.debug.OnUpdate();
    }

    private void Test()
    {
        Mgr.Save.AllCharacterData.Unlock(0);
        Mgr.Save.AllCharacterData.Unlock(1);
        Mgr.Save.AllCharacterData.Unlock(2);
        Mgr.Save.AllCharacterData.Unlock(3);
        Mgr.Save.AllCharacterData.Unlock(4);
        Mgr.Save.AllCharacterData.Unlock(5);
        Mgr.Save.AllCharacterData.Unlock(6);
        Mgr.Save.AllCharacterData.Unlock(7);
        Mgr.Save.RewardStockData.GetStock(RewardDisplayType.Currency).Add(1000001, 1000, 99999);

        // テスト用: Useアイテム300種を生成
        var useStock = Mgr.Save.RewardStockData.GetStock(RewardDisplayType.Use);
        for (int i = 0; i < 300; i++)
        {
            int rewardId = RewardBase.Multiplier * (int)RewardType.Use + i + 1;
            var master = new UseItemMaster
            {
                key = $"test-use-{i + 1:D3}",
                rewardId = rewardId,
            };
            Mgr.Master.AllRewardMasters[rewardId] = master;
            useStock.Add(rewardId, 10, master.MaxStock);
        }
    }
}
