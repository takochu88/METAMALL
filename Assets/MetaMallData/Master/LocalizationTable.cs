using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationTable : ScriptableObject, IReadOnlyList<LocalizationMaster>
{
    [SerializeField] private List<LocalizationMaster> rows = new List<LocalizationMaster>();

    public int Count => rows.Count;
    public LocalizationMaster this[int index] => rows[index];

    public IEnumerator<LocalizationMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
