using System;

[Serializable]
public class AbilityData
{
    public int masterId;

    public AbilityMaster Master => Mgr.Master.abilityTable[masterId];

    public AbilityData(int masterId)
    {
        this.masterId = masterId;
    }
}
