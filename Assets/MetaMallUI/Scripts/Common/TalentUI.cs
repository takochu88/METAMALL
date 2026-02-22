using UnityEngine;

public class TalentUI : MonoBehaviour
{
    [SerializeField] TalentRowUI[] rows;

    public void Setup()
    {
        for (int i = 0; i < rows.Length; i++)
        {
            TalentRowUI row = rows[i];
            row.Setup();
        }
    }

    public void UpdateUI(FighterData data)
    {
        int[] totalBonus = data.GetTotalBonuses();
        
        for (int i = 0; i < rows.Length; i++)
        {
            TalentRowUI row = rows[i];
            row.UpdateUI(totalBonus[i]);
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
