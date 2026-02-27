using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeTable : ScriptableObject, IReadOnlyList<ArcadeMaster>
{
    [SerializeField] private List<ArcadeMaster> rows = new List<ArcadeMaster>();

    public int Count => rows.Count;
    public ArcadeMaster this[int index] => rows[index];

    public IEnumerator<ArcadeMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
