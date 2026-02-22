using System;

[Serializable]
public class FighterData
{
    private const int BonusTypeCount = 4;
    private const int TalentCount = 8;
    private const int StatusCount = 12;

    public int masterId;
    public int[][] talentBonuses; // [BonusType][TalentType] = int[4][8]
    public int[] status; // [StatusType] = int[12]

    /// <summary> 全ボーナス配列を初期化 </summary>
    public void InitTalents()
    {
        talentBonuses = new int[BonusTypeCount][];
        for (int i = 0; i < BonusTypeCount; i++)
            talentBonuses[i] = new int[TalentCount];
    }

    /// <summary> ステータス配列を初期化 </summary>
    public void InitStatus()
    {
        status = new int[StatusCount];
    }

    /// <summary> ステータス値を取得 </summary>
    public int GetStatus(StatusType type) => status[(int)type];

    /// <summary> ステータス値を設定 </summary>
    public void SetStatus(StatusType type, int value) => status[(int)type] = value;

    /// <summary> enum指定でボーナス配列を返す </summary>
    public int[] GetBonus(TalentBonusType type) => talentBonuses[(int)type];

    /// <summary> 個別ボーナス値を取得 </summary>
    public int GetBonusValue(TalentType talent, TalentBonusType bonus)
        => talentBonuses[(int)bonus][(int)talent];

    /// <summary> 個別ボーナス値を設定 </summary>
    public void SetBonusValue(TalentType talent, TalentBonusType bonus, int value)
        => talentBonuses[(int)bonus][(int)talent] = value;

    /// <summary> 指定TalentTypeの全ボーナス合計値（4種合算） </summary>
    public int GetTotalBonus(TalentType talent)
    {
        int t = (int)talent, sum = 0;
        for (int b = 0; b < BonusTypeCount; b++) sum += talentBonuses[b][t];
        return sum;
    }

    /// <summary> 全TalentTypeのボーナス合計をint[8]で返す </summary>
    public int[] GetTotalBonuses()
    {
        var totals = new int[TalentCount];
        for (int t = 0; t < TalentCount; t++)
            for (int b = 0; b < BonusTypeCount; b++)
                totals[t] += talentBonuses[b][t];
        return totals;
    }

    public void GetBaseStatus()
    {
    }
}
