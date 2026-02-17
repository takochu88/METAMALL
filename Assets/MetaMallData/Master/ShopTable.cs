using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTable : ScriptableObject, IReadOnlyList<ShopMaster>
{
    [SerializeField] private List<ShopMaster> rows = new List<ShopMaster>();

    public int Count => rows.Count;
    public ShopMaster this[int index] => rows[index];

    public IEnumerator<ShopMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
