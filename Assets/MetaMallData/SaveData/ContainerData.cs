using System;
using System.Collections.Generic;

[Serializable]
public class ContainerData
{
    public int masterId;
    public List<RewardEntry> rewards = new();
    public bool isOpened;

    public ContainerMaster Master => Mgr.Master.containerTable[masterId];

    public ContainerData(int masterId)
    {
        this.masterId = masterId;
    }
}

[Serializable]
public class RewardEntry
{
    public int rewardId;
    public int amount;
}
