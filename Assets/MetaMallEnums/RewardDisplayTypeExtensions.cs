public static class RewardDisplayTypeExtensions
{
    /// <summary> インベントリに表示する DisplayType 一覧 </summary>
    private static readonly RewardDisplayType[] InventoryTypes =
    {
        RewardDisplayType.Currency,
        RewardDisplayType.Ticket,
        RewardDisplayType.Other,
        RewardDisplayType.Use,
    };

    public static RewardDisplayType[] GetInventoryTypes() => InventoryTypes;

    /// <summary> ローカライズされた表示名を返す（キー: display-type-{name}） </summary>
    public static string ToLocalizedName(this RewardDisplayType type)
    {
        return Mgr.Local.Get($"display-type-{type.ToString().ToLower()}");
    }
}
