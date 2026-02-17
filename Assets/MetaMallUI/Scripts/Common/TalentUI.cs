using System;
using UnityEngine;

public class TalentUI : MonoBehaviour
{
    [SerializeField] TalentRowUI[] rows;

    /// <summary>
    /// フルコントロール版。全行のデータとコールバックを一括設定。
    /// </summary>
    public void SetData(int[] baseTalents, int[] bonusTalents, int maxValue, bool[] canUpgrade, Action<int> onUpgrade)
    {
        for (int i = 0; i < rows.Length; i++)
        {
            var type = (TalentType)i;
            string name = Mgr.Local.Get($"talent-{type}");
            int baseVal = baseTalents[i];
            int bonus = bonusTalents != null ? bonusTalents[i] : 0;
            bool upgrade = canUpgrade != null && canUpgrade[i];

            rows[i].SetData(name, baseVal, bonus, maxValue, upgrade);
            rows[i].SetOnUpgrade(i, onUpgrade);
        }
    }

    /// <summary>
    /// ボーナスなし閲覧用の簡易版。
    /// </summary>
    public void SetData(FighterMaster fighter, int maxValue)
    {
        for (int i = 0; i < rows.Length; i++)
        {
            var type = (TalentType)i;
            string name = Mgr.Local.Get($"talent-{type}");
            int baseVal = fighter.GetTalent(type);

            rows[i].SetData(name, baseVal, 0, maxValue, false);
        }
    }

    /// <summary>
    /// 1行だけ更新。
    /// </summary>
    public void UpdateRow(TalentType type, int baseValue, int bonusValue, int maxValue, bool canUpgrade)
    {
        int i = (int)type;
        string name = Mgr.Local.Get($"talent-{type}");
        rows[i].SetData(name, baseValue, bonusValue, maxValue, canUpgrade);
    }
}
