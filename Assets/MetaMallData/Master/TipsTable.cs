using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipsTable : ScriptableObject, IReadOnlyList<TipsMaster>
{
    [SerializeField] private List<TipsMaster> rows = new List<TipsMaster>();

    public int Count => rows.Count;
    public TipsMaster this[int index] => rows[index];

    public IEnumerator<TipsMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
