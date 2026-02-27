using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionTable : ScriptableObject, IReadOnlyList<CollectionMaster>
{
    [SerializeField] private List<CollectionMaster> rows = new List<CollectionMaster>();

    public int Count => rows.Count;
    public CollectionMaster this[int index] => rows[index];

    public IEnumerator<CollectionMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
