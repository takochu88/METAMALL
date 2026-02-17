using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelTable : ScriptableObject, IReadOnlyList<ParallelMaster>
{
    [SerializeField] private List<ParallelMaster> rows = new List<ParallelMaster>();

    public int Count => rows.Count;
    public ParallelMaster this[int index] => rows[index];

    public IEnumerator<ParallelMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
