using System;

public static class MetaMallUtils
{
    public static string ToRemainingTimeLabel(TimeSpan remaining)
    {
        // 期限切れ
        if (remaining.TotalSeconds <= 0)
            return Mgr.Local.Get("ui-expired");

        // 1日以上：日+時間
        if (remaining.TotalDays >= 1)
        {
            int days = (int)remaining.TotalDays;
            int hours = remaining.Hours; // 0-23
            return Mgr.Local.Format("ui-expire-in-days-hours", days, hours);
        }

        // 1日未満：時間のみ（1時間未満でも 0時間）
        int totalHours = (int)remaining.TotalHours; // 0..23
        return Mgr.Local.Format("ui-expire-in-hours", totalHours);
    }

    /// <summary>
    /// 終了時刻から残り時間文字列を取得する。
    /// - expireAt が null の場合は「無期限」扱い
    /// - showNoExpiration=false の場合、無期限は null を返す
    /// - 期限切れの場合は null を返す
    /// </summary>
    public static string ToRemainingTimeString(DateTime? expireAt, bool showNoExpiration = true)
    {
        // 無期限
        if (!expireAt.HasValue)
            return showNoExpiration ? Mgr.Local.Get("ui-no-expiration") : null;

        var remaining = expireAt.Value - DateTime.UtcNow;

        // 期限切れ
        if (remaining.TotalSeconds <= 0)
            return null;

        return ToRemainingTimeLabel(remaining);
    }
}
