using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionTable : ScriptableObject, IReadOnlyList<MissionMaster>
{
    [SerializeField] private List<MissionMaster> rows = new List<MissionMaster>();

    public int Count => rows.Count;
    public MissionMaster this[int index] => rows[index];

    public IEnumerator<MissionMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
