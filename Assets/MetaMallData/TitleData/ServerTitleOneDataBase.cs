using System;
using System.Globalization;

/// <summary>
/// PlayFab TitleNews Body JSON の共通基底クラス。
/// PlayFab メタデータ（NewsId, Title, Timestamp）もパース後に設定される。
/// </summary>
[Serializable]
public abstract class ServerTitleOneDataBase
{
    public string body;
    public string startAt;
    public string expireAt;

    // PlayFab メタデータ（JSON 外、パース後に設定）
    [NonSerialized] public string NewsId;
    [NonSerialized] public string Title;
    [NonSerialized] public DateTime Timestamp;

    private DateTime? parsedStartAt;
    private DateTime? parsedExpireAt;
    private bool parsed;

    public DateTime? StartAtUtc => Parse().parsedStartAt;
    public DateTime? ExpireAtUtc => Parse().parsedExpireAt;

    public bool IsStarted => !parsedStartAt.HasValue || DateTime.UtcNow >= parsedStartAt.Value;
    public bool IsExpired => parsedExpireAt.HasValue && DateTime.UtcNow >= parsedExpireAt.Value;
    public bool IsActive => IsStarted && !IsExpired;

    private ServerTitleOneDataBase Parse()
    {
        if (parsed) return this;
        parsed = true;
        parsedStartAt = TryParseDateTime(startAt);
        parsedExpireAt = TryParseDateTime(expireAt);
        return this;
    }

    private static DateTime? TryParseDateTime(string value)
    {
        if (string.IsNullOrEmpty(value)) return null;
        if (DateTime.TryParse(value, null, DateTimeStyles.RoundtripKind, out var result))
            return result;
        return null;
    }
}
