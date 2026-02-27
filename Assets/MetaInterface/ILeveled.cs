public interface ILeveled
{
    LeveledType LeveledType { get; }

    // レベル範囲
    int MinLevel { get; }
    int MaxLevel { get; }

    // 現在レベル
    int CurrentLevel { get; }

    // いまの値（例：経験値、進捗ポイント、スコアなど）
    int Value { get; }

    // レベルアップ可能か（Max到達、必要値不足などを内部判断）
    bool CanLevelUp { get; }

    // レベルアップ実行（成功したら true）
    bool TryLevelUp();

    // レベルアップ後に呼ばれる通知（UI更新などで使う）
    event System.Action<ILeveled> OnLevelUp;
}
