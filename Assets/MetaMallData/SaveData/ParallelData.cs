using System;

[Serializable]
public class ParallelData
{
    public int masterId;

    public ParallelMaster Master => Mgr.Master.parallelTable[masterId];

    public ParallelData(int masterId)
    {
        this.masterId = masterId;
    }
}
