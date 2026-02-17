using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigTable : ScriptableObject, IReadOnlyList<ConfigMaster>
{
    [SerializeField] private List<ConfigMaster> rows = new List<ConfigMaster>();

    public int Count => rows.Count;
    public ConfigMaster this[int index] => rows[index];

    public IEnumerator<ConfigMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
