using System;

public enum DebugCategory
{
    Character,
    Battle,
    Reward,
    Other,
}

public class DebugCommand
{
    public string Name { get; }
    public DebugCategory Category { get; }
    public Action<Main> Execute { get; }

    public DebugCommand(string name, DebugCategory category, Action<Main> execute)
    {
        Name = name;
        Category = category;
        Execute = execute;
    }
}
