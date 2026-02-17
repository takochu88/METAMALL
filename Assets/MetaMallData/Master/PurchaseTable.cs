using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseTable : ScriptableObject, IReadOnlyList<PurchaseMaster>
{
    [SerializeField] private List<PurchaseMaster> rows = new List<PurchaseMaster>();

    public int Count => rows.Count;
    public PurchaseMaster this[int index] => rows[index];

    public IEnumerator<PurchaseMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
