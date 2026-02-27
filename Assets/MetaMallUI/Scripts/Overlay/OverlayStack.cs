using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class OverlayStack : MonoBehaviour
{
    [SerializeField] int baseSortingOrder = 100;

    [SerializeField, ReadOnly] List<OverlayUIBase> stack = new();
    [SerializeField, ReadOnly] int currentMaxSortingOrder = -1;

    // ── プロパティ ───────────────────────────
    public OverlayUIBase Current => stack.Count > 0 ? stack[^1] : null;
    public bool IsAnyShown => stack.Count > 0;
    public int ActiveCount => stack.Count;

    public MailUI mailUI;
    public CustomCharacterUI customCharacterUI;
    public FormationUI formationUI;
    public DebugPanelUI debugPanelUI;
    public RewardDetailUI rewardDetailUI;
    public RewardUseUI rewardUseUI;
    public GetRewardUI getRewardUI;
    public ConfirmUI confirmUI;
    public CautionUI cautionUI;
    public InventoryUI inventoryUI;
    public ConfigUI configUI;
    public MetaTaskUI metaTaskUI;
    public PatrolUI patrolUI;
    [FormerlySerializedAs("arcadeUI")] public ArcadeMenuUI arcadeMenuUI;
    public StabilityDetailUI stabilityDetailUI;
    public TipsUI tipsUI;
    public CollectionUI collectionUI;
    public MissionUI missionUI;
    public ShopUI shopUI;
    public SeasonPassUI seasonPassUI;
    public LoginBonusUI loginBonusUI;

    // ── 初期化 ───────────────────────────────
    void Awake()
    {
        var overlays = GetComponentsInChildren<OverlayUIBase>(true);
        foreach (var ui in overlays)
            ui.Setup(this);
    }

    // ── Push（OverlayUIBase.Show()から呼ばれる）
    public void Push(OverlayUIBase ui)
    {
        int newOrder = Mathf.Max(
            baseSortingOrder + stack.Count * 10,
            currentMaxSortingOrder + 10
        );
        currentMaxSortingOrder = newOrder;
        ui.SetSortingOrder(newOrder);
        stack.Add(ui);
    }

    // ── Remove（OverlayUIBase.Hide()完了後に呼ばれる）
    public void Remove(OverlayUIBase ui)
    {
        if (stack.Count > 0 && ui == stack[^1])
        {
            if (stack.Count >= 2)
            {
                stack[^2].OnActivated();
                currentMaxSortingOrder = stack[^2].SortingOrder;
            }
            else
            {
                currentMaxSortingOrder = -1;
            }
        }
        stack.Remove(ui);
    }

    // ── 便利メソッド ─────────────────────────
    public void HideCurrent() => Current?.Hide();

    public void HideAll()
    {
        for (int i = stack.Count - 1; i >= 0; i--)
            stack[i].Hide();
    }

    public bool IsShowingUI<T>() where T : OverlayUIBase
    {
        return stack.Exists(ui => ui is T);
    }

    public T GetOverlay<T>() where T : OverlayUIBase
    {
        return GetComponentInChildren<T>(true);
    }

}
