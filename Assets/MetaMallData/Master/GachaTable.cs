using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaTable : ScriptableObject, IReadOnlyList<GachaMaster>
{
    [SerializeField] private List<GachaMaster> rows = new List<GachaMaster>();

    public int Count => rows.Count;
    public GachaMaster this[int index] => rows[index];

    public IEnumerator<GachaMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
