using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItemTable : ScriptableObject, IReadOnlyList<UseItemMaster>
{
    [SerializeField] private List<UseItemMaster> rows = new List<UseItemMaster>();

    public int Count => rows.Count;
    public UseItemMaster this[int index] => rows[index];

    public IEnumerator<UseItemMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
