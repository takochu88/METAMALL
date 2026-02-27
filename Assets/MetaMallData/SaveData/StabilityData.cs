using System;

/// <summary>
/// スタミナ（Stability）管理データ。
/// PlayFab Virtual Currency (ST) のクライアント側キャッシュ。
/// サーバー取得値を基に、ローカルで時間経過回復をオンザフライ計算する。
/// </summary>
public class StabilityData
{
    public const string CurrencyCode = "ST";
    private const int recoverySeconds = 300;

    private int storedAmount;
    private int max = 100;
    private DateTime baseTime;
    private bool initialized;

    /// <summary> 値が変化したときに発火 </summary>
    public event Action OnChanged;

    public int Max => max;

    /// <summary> 経過時間ベースで現在量を計算（max超過分はそのまま返す） </summary>
    public int Amount
    {
        get
        {
            if (!initialized) return max;
            if (storedAmount >= max) return storedAmount;
            double elapsed = (DateTime.UtcNow - baseTime).TotalSeconds;
            int recovered = (int)(elapsed / recoverySeconds);
            return Math.Min(storedAmount + recovered, max);
        }
    }

    public bool IsFull => Amount >= max;

    /// <summary>
    /// サーバーから取得した値で更新する。
    /// secondsToRecharge（次の1回復までの残り秒数）から baseTime を逆算し、
    /// 以降はローカルで回復をオンザフライ計算する。
    /// </summary>
    public void Apply(int amount, int max, int secondsToRecharge)
    {
        storedAmount = amount;
        this.max = max;
        baseTime = DateTime.UtcNow - TimeSpan.FromSeconds(recoverySeconds - secondsToRecharge);
        initialized = true;
        OnChanged?.Invoke();
    }

    /// <summary> 消費/追加後の残高で更新する </summary>
    public void SetBalance(int newBalance)
    {
        storedAmount = newBalance;
        baseTime = DateTime.UtcNow;
        OnChanged?.Invoke();
    }

    public bool CanConsume(int cost) => cost > 0 && Amount >= cost;

    /// <summary> 次の1回復までの残り時間 </summary>
    public TimeSpan GetTimeToNext()
    {
        if (!initialized || IsFull) return TimeSpan.Zero;
        double elapsed = (DateTime.UtcNow - baseTime).TotalSeconds;
        double remainder = elapsed % recoverySeconds;
        return TimeSpan.FromSeconds(recoverySeconds - remainder);
    }

    /// <summary> 全回復までの残り時間 </summary>
    public TimeSpan GetTimeToFull()
    {
        int current = Amount;
        if (current >= max) return TimeSpan.Zero;
        int deficit = max - current;
        var toNext = GetTimeToNext();
        return toNext + TimeSpan.FromSeconds((deficit - 1) * recoverySeconds);
    }
}
