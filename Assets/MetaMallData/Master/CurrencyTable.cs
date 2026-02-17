using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyTable : ScriptableObject, IReadOnlyList<CurrencyMaster>
{
    [SerializeField] private List<CurrencyMaster> rows = new List<CurrencyMaster>();

    public int Count => rows.Count;
    public CurrencyMaster this[int index] => rows[index];

    public IEnumerator<CurrencyMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
