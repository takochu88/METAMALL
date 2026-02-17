using System.Collections.Generic;
using System.Linq;

public static partial class DebugCommandList
{
    private static readonly List<DebugCommand> commands = new();

    static DebugCommandList()
    {
        RegisterCharacterCommands();
        RegisterBattleCommands();
        RegisterRewardCommands();
        RegisterOtherCommands();
    }

    public static IReadOnlyList<DebugCommand> All => commands;

    public static List<DebugCommand> GetFiltered(string filter)
    {
        if (string.IsNullOrEmpty(filter))
            return commands;

        var lower = filter.ToLowerInvariant();
        return commands.Where(c => c.Name.ToLowerInvariant().Contains(lower)).ToList();
    }

    private static void Add(string name, DebugCategory category, System.Action<Main> execute)
    {
        commands.Add(new DebugCommand(name, category, execute));
    }
}
