using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMenuTable : ScriptableObject, IReadOnlyList<SubMenuMaster>
{
    [SerializeField] private List<SubMenuMaster> rows = new List<SubMenuMaster>();

    public int Count => rows.Count;
    public SubMenuMaster this[int index] => rows[index];

    public IEnumerator<SubMenuMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
