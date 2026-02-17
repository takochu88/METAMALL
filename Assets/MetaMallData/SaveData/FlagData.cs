using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class FlagData
{
    public HashSet<FlagType> flags;

    public FlagData()
    {
        flags = new HashSet<FlagType>();
    }
    
    public bool IsTrue(FlagType type)
    {
        return flags.Contains(type);
    }

    public bool IsFalse(FlagType type)
    {
        return !flags.Contains(type);
    }

    [Button]
    public void SetOn(FlagType type)
    {
        flags.Add(type);
    }

    [Button]
    public void SetOff(FlagType type)
    {
        flags.Remove(type);
    }

    [Button]
    public void LogIf(FlagType type)
    {
        Debug.Log($"{type}={IsTrue(type)}");
    }
}
