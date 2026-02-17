using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MasterManager : MonoBehaviour
{
    [BoxGroup("Fighter")]    public CharacterTable characterTable;
    [BoxGroup("Fighter")]    public EnemyTable enemyTable;
    [BoxGroup("Fighter")]    public ParallelTable parallelTable;
    [BoxGroup("Fighter")]    public EquipmentTable equipmentTable;
    [BoxGroup("Fighter")]    public AbilityTable abilityTable;

    [BoxGroup("リワード")]    public CurrencyTable currencyTable;
    [BoxGroup("リワード")]    public GachaTicketTable gachaTicketTable;
    [BoxGroup("リワード")]    public StageTicketTable stageTicketTable;
    [BoxGroup("リワード")]    public UnlockCharacterTable unlockCharacterTable;
    [BoxGroup("リワード")]    public OtherRewardTable otherRewardTable;

    [BoxGroup("システム")]    public TipsTable tipsTable;
    [BoxGroup("システム")]    public ConfigTable configTable;
    [BoxGroup("システム")]    public NoticeTable noticeTable;

    [BoxGroup("ショップ")]    public ShopTable shopTable;
    [BoxGroup("ショップ")]    public PurchaseTable purchaseTable;
    [BoxGroup("ショップ")]    public GachaTable gachaTable;

    [BoxGroup("メニュー")]    public MainMenuTable mainMenuTable;
    [BoxGroup("メニュー")]    public SubMenuTable subMenuTable;

    [BoxGroup("コンテンツ")]  public MallTable mallTable;
    [BoxGroup("コンテンツ")]  public ContainerTable containerTable;
    [BoxGroup("コンテンツ")]  public MissionTable missionTable;
    [BoxGroup("コンテンツ")]  public GameEventTable gameEventTable;

    Dictionary<int, RewardBase> allRewardMasters;
    public Dictionary<int, RewardBase> AllRewardMasters => allRewardMasters;

    public void Init()
    {
        BuildAllRewardMasters();
    }

    void BuildAllRewardMasters()
    {
        allRewardMasters = new Dictionary<int, RewardBase>();
        AddRange(currencyTable);
        AddRange(gachaTicketTable);
        AddRange(stageTicketTable);
        AddRange(unlockCharacterTable);
        AddRange(otherRewardTable);
    }

    void AddRange<T>(IReadOnlyList<T> table) where T : RewardBase
    {
        if (table == null) return;
        for (int i = 0; i < table.Count; i++)
            allRewardMasters[table[i].rewardId] = table[i];
    }

    public bool TryGetReward(int rewardId, out RewardBase reward)
    {
        return AllRewardMasters.TryGetValue(rewardId, out reward);
    }
}
