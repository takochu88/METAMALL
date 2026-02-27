using System;

public class ArcadeData : ILeveled
{
    public LeveledType LeveledType { get; }
    public int MinLevel { get; } = 1;
    public int MaxLevel { get; } = 100;
    public int CurrentLevel { get; }
    public int Value { get; }
    public bool CanLevelUp { get; }

    public ArcadeData()
    {
        CurrentLevel = MinLevel;
    }
 
    public bool TryLevelUp()
    {
        throw new NotImplementedException();
    }

    public event Action<ILeveled> OnLevelUp;
}
