using System;
using System.Collections.Generic;
using System.Reflection;
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
    [BoxGroup("リワード")]    public UseItemTable useItemTable;

    [BoxGroup("システム")]    public TipsTable tipsTable;
    [BoxGroup("システム")]    public ConfigTable configTable;
    [BoxGroup("システム")]    public NoticeTable noticeTable;

    [BoxGroup("ショップ")]    public ShopTable shopTable;
    [BoxGroup("ショップ")]    public PurchaseTable purchaseTable;
    [BoxGroup("ショップ")]    public GachaTable gachaTable;

    [BoxGroup("メニュー")]    public MainMenuTable mainMenuTable;
    [BoxGroup("メニュー")]    public SubMenuTable subMenuTable;

    [BoxGroup("コンテンツ")]  public MallTable mallTable;
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

        var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (!IsRewardTable(field.FieldType)) continue;
            if (field.GetValue(this) is not System.Collections.IEnumerable enumerable) continue;
            foreach (var item in enumerable)
            {
                if (item is RewardBase reward)
                    allRewardMasters[reward.rewardId] = reward;
            }
        }
    }

    static bool IsRewardTable(Type type)
    {
        foreach (var iface in type.GetInterfaces())
        {
            if (!iface.IsGenericType) continue;
            if (iface.GetGenericTypeDefinition() != typeof(IReadOnlyList<>)) continue;
            if (typeof(RewardBase).IsAssignableFrom(iface.GetGenericArguments()[0]))
                return true;
        }
        return false;
    }

    public bool TryGetReward(int rewardId, out RewardBase reward)
    {
        if (AllRewardMasters.TryGetValue(rewardId, out reward))
            return true;

        Debug.LogError($"[Master] rewardId={rewardId} がマスターに存在しません");
        return false;
    }

    public List<RewardBase> GetRewardsByDisplayType(RewardDisplayType type)
    {
        var result = new List<RewardBase>();
        foreach (var reward in allRewardMasters.Values)
        {
            if (reward.DisplayType == type)
                result.Add(reward);
        }
        return result;
    }

    public bool TryGetMenuBase(MenuType menuType, out MenuBase menuBase)
    {
        if (mainMenuTable != null)
        {
            for (int i = 0; i < mainMenuTable.Count; i++)
            {
                var m = mainMenuTable[i];
                if (m.menuType == menuType)
                {
                    menuBase = m;
                    return true;
                }
            }
        }

        if (subMenuTable != null)
        {
            for (int i = 0; i < subMenuTable.Count; i++)
            {
                var s = subMenuTable[i];
                if (s.menuType == menuType)
                {
                    menuBase = s;
                    return true;
                }
            }
        }
        
        menuBase = null;
        return false;
    }
}
