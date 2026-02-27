using System.Collections.Generic;

/// <summary>
/// セッション中のみ保持される一時データ（永続化しない）。
/// </summary>
public class SessionData
{
    private readonly Dictionary<MenuType, int> selectedIndices = new();

    public int GetIndex(MenuType key, int defaultValue = 0)
    {
        return selectedIndices.TryGetValue(key, out var v) ? v : defaultValue;
    }

    public void SetIndex(MenuType key, int value)
    {
        selectedIndices[key] = value;
    }
}
