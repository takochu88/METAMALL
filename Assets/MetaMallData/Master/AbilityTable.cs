using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTable : ScriptableObject, IReadOnlyList<AbilityMaster>
{
    [SerializeField] private List<AbilityMaster> rows = new List<AbilityMaster>();

    public int Count => rows.Count;
    public AbilityMaster this[int index] => rows[index];

    public IEnumerator<AbilityMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
