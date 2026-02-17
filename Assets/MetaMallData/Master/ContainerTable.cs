using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerTable : ScriptableObject, IReadOnlyList<ContainerMaster>
{
    [SerializeField] private List<ContainerMaster> rows = new List<ContainerMaster>();

    public int Count => rows.Count;
    public ContainerMaster this[int index] => rows[index];

    public IEnumerator<ContainerMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
