using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaTaskTable : ScriptableObject, IReadOnlyList<MetaTaskMaster>
{
    [SerializeField] private List<MetaTaskMaster> rows = new List<MetaTaskMaster>();

    public int Count => rows.Count;
    public MetaTaskMaster this[int index] => rows[index];

    public IEnumerator<MetaTaskMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
