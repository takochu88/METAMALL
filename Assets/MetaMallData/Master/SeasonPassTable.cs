using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonPassTable : ScriptableObject, IReadOnlyList<SeasonPassMaster>
{
    [SerializeField] private List<SeasonPassMaster> rows = new List<SeasonPassMaster>();

    public int Count => rows.Count;
    public SeasonPassMaster this[int index] => rows[index];

    public IEnumerator<SeasonPassMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
