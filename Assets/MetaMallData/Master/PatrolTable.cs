using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolTable : ScriptableObject, IReadOnlyList<PatrolMaster>
{
    [SerializeField] private List<PatrolMaster> rows = new List<PatrolMaster>();

    public int Count => rows.Count;
    public PatrolMaster this[int index] => rows[index];

    public IEnumerator<PatrolMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
