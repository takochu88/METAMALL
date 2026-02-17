using System;
using System.Collections.Generic;

[Serializable]
public class CharacterData : FighterData
{
    public string unlockedAt; // ISO 8601 (例: "2026-02-16T12:00:00")
    public List<ParallelData> parallels = new();
    public List<EquipmentData> equipments = new();
    public List<AbilityData> abilities = new();

    /// <summary> Master参照 </summary>
    public CharacterMaster Master => Mgr.Master.characterTable[masterId];

    public CharacterData(int masterId)
    {
        this.masterId = masterId;
        InitTalents();
    }

    /// <summary> アンロック日時を記録 </summary>
    public void Unlock()
    {
        unlockedAt = DateTime.Now.ToString("s"); // ISO 8601
    }
}
