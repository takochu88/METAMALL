using System;

[Serializable]
public class EquipmentData
{
    public int masterId;

    public EquipmentMaster Master => Mgr.Master.equipmentTable[masterId];

    public EquipmentData(int masterId)
    {
        this.masterId = masterId;
    }
}
