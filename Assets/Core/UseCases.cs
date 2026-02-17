using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UseCases : MonoBehaviour
{
    [BoxGroup("コア")]    public UseCase_Config config;
    [BoxGroup("コア")]    public UseCase_Mall mall;

    [BoxGroup("システム")] public UseCase_Popup popup;
    [BoxGroup("システム")] public UseCase_Tips tips;
    [BoxGroup("システム")] public UseCase_Notice notice;

    [BoxGroup("ショップ")] public UseCase_Shop shop;
    [BoxGroup("ショップ")] public UseCase_Purchase purchase;
    [BoxGroup("ショップ")] public UseCase_Gacha gacha;

    [FormerlySerializedAs("menu")] [BoxGroup("メニュー")]   public UseCase_HandleMenu handleMenu;
    [BoxGroup("メニュー")]   public UseCase_TopMenu topMenu;
    [BoxGroup("メニュー")]   public UseCase_Stage stage;

    [BoxGroup("コンテンツ")] public UseCase_Container container;
    [BoxGroup("コンテンツ")] public UseCase_Mission mission;
    [BoxGroup("コンテンツ")] public UseCase_GameEvent gameEvent;

    [BoxGroup("キャラクター")] public UseCase_CustomCharacter customCharacter;
    [BoxGroup("キャラクター")] public UseCase_Formation formation;
    [BoxGroup("キャラクター")] public UseCase_Parallel parallel;
    [BoxGroup("キャラクター")] public UseCase_Equipment equipment;
    [BoxGroup("キャラクター")] public UseCase_Ability ability;

    [BoxGroup("アイテム")]   public UseCase_Reward reward;
    [BoxGroup("アイテム")]   public UseCase_DisplayItem displayItem;

    [BoxGroup("デバッグ")]   public UseCase_Debug debug;

    public void Setup(Main main)
    {
        popup.Setup(main);
        tips.Setup(main);
        config.Setup(main);
        shop.Setup(main);
        purchase.Setup(main);
        gacha.Setup(main);
        mall.Setup(main);
        notice.Setup(main);
        handleMenu.Setup(main);
        topMenu.Setup(main);
        stage.Setup(main);
        container.Setup(main);
        mission.Setup(main);
        gameEvent.Setup(main);
        customCharacter.Setup(main);
        formation.Setup(main);
        parallel.Setup(main);
        equipment.Setup(main);
        ability.Setup(main);
        reward.Setup(main);
        displayItem.Setup(main);
        debug.Setup(main);
    }

#if UNITY_EDITOR
    [Button("全UseCaseを生成・登録", ButtonSizes.Large)]
    [PropertyOrder(100)]
    private void GenerateAllUseCases()
    {
        int order = 0;
        // スクリプトのフィールド順 = ヒエラルキー順
        config    = GetOrCreate<UseCase_Config>("Config", order++);
        mall      = GetOrCreate<UseCase_Mall>("Mall", order++);
        popup     = GetOrCreate<UseCase_Popup>("Popup", order++);
        tips      = GetOrCreate<UseCase_Tips>("Tips", order++);
        notice    = GetOrCreate<UseCase_Notice>("Notice", order++);
        shop      = GetOrCreate<UseCase_Shop>("Shop", order++);
        purchase  = GetOrCreate<UseCase_Purchase>("Purchase", order++);
        gacha     = GetOrCreate<UseCase_Gacha>("Gacha", order++);
        handleMenu      = GetOrCreate<UseCase_HandleMenu>("Menu", order++);
        topMenu   = GetOrCreate<UseCase_TopMenu>("TopMenu", order++);
        stage     = GetOrCreate<UseCase_Stage>("Stage", order++);
        container = GetOrCreate<UseCase_Container>("Container", order++);
        mission   = GetOrCreate<UseCase_Mission>("Mission", order++);
        gameEvent = GetOrCreate<UseCase_GameEvent>("GameEvent", order++);
        customCharacter = GetOrCreate<UseCase_CustomCharacter>("CustomCharacter", order++);
        formation = GetOrCreate<UseCase_Formation>("Formation", order++);
        parallel  = GetOrCreate<UseCase_Parallel>("Parallel", order++);
        equipment = GetOrCreate<UseCase_Equipment>("Equipment", order++);
        ability   = GetOrCreate<UseCase_Ability>("Ability", order++);
        reward    = GetOrCreate<UseCase_Reward>("Reward", order++);
        displayItem = GetOrCreate<UseCase_DisplayItem>("DisplayItem", order++);
        debug       = GetOrCreate<UseCase_Debug>("Debug", order++);

        EditorUtility.SetDirty(this);
        Debug.Log("[UseCases] 全UseCaseを生成・登録しました");
    }

    private T GetOrCreate<T>(string objName, int siblingIndex) where T : MonoBehaviour
    {
        var existing = GetComponentInChildren<T>(true);
        if (existing != null)
        {
            existing.transform.SetSiblingIndex(siblingIndex);
            return existing;
        }

        var child = new GameObject(objName);
        child.transform.SetParent(transform);
        child.transform.SetSiblingIndex(siblingIndex);
        Undo.RegisterCreatedObjectUndo(child, $"Create {objName}");
        return child.AddComponent<T>();
    }
#endif
}
