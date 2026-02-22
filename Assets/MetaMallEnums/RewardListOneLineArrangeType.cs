using Sirenix.OdinInspector;

/// <summary>
/// 1行に収まる場合のアイコン揃え方向
/// </summary>
public enum RewardListOneLineArrangeType
{
    /// <summary>中央揃え</summary>
    [LabelText("中央揃え")]
    Center = 0,

    /// <summary>左揃え</summary>
    [LabelText("左揃え")]
    Left = 1,

    /// <summary>右揃え</summary>
    [LabelText("右揃え")]
    Right = 2,
}
