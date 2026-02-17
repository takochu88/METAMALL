using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaTicketTable : ScriptableObject, IReadOnlyList<GachaTicketMaster>
{
    [SerializeField] private List<GachaTicketMaster> rows = new List<GachaTicketMaster>();

    public int Count => rows.Count;
    public GachaTicketMaster this[int index] => rows[index];

    public IEnumerator<GachaTicketMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
