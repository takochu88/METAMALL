using Sirenix.OdinInspector;

/// <summary>
/// RewardList のアイコン配置方法
/// </summary>
public enum RewardListArrangeType
{
    /// <summary>親の幅に合わせて自動で折り返す。1行あたりの数を自動計算する</summary>
    [LabelText("親のサイズを超えたら折り返し")]
    WrapByParentSize = 0,

    /// <summary>1行を横スクロールで表示する。全アイテムを1行に並べる</summary>
    [LabelText("1行スクロール")]
    SingleRowInfiniteScroll = 1,

    /// <summary>1行あたりのアイテム数を指定して折り返す</summary>
    [LabelText("行あたり指定数")]
    WrapByItemsPerRow = 2,
}
