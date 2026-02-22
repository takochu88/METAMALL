using UnityEngine;

public enum GetRewardType
{
    Get = 0,
    OverFlow =1,
    Removed =2,
}

static class GetRewardTypeExtensions
{
    public static string GetTitle(this GetRewardType type)
    {
        return Mgr.Local.Get("ui-title-" + type);
    }

    public static Color GetColor(this GetRewardType type)
    {
        // 好みで差し替えてOK
        return type switch
        {
            GetRewardType.Get => Color.yellow,
            GetRewardType.OverFlow => Color.orange,
            GetRewardType.Removed => Color.red,
            _ => Color.white,
        };
    }
}