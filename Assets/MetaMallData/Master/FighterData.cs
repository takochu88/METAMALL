public abstract class FighterMaster : MasterBase
{
    public int[] talents;

    public int GetTalent(TalentType type) => talents[(int)type];
    public void SetTalent(TalentType type, int value) => talents[(int)type] = value;
}
