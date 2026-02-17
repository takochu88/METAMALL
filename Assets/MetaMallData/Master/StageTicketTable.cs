using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTicketTable : ScriptableObject, IReadOnlyList<StageTicketMaster>
{
    [SerializeField] private List<StageTicketMaster> rows = new List<StageTicketMaster>();

    public int Count => rows.Count;
    public StageTicketMaster this[int index] => rows[index];

    public IEnumerator<StageTicketMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
