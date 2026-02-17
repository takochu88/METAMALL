using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallTable : ScriptableObject, IReadOnlyList<MallMaster>
{
    [SerializeField] private List<MallMaster> rows = new List<MallMaster>();

    public int Count => rows.Count;
    public MallMaster this[int index] => rows[index];

    public IEnumerator<MallMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
