using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTable : ScriptableObject, IReadOnlyList<EnemyMaster>
{
    [SerializeField] private List<EnemyMaster> rows = new List<EnemyMaster>();

    public int Count => rows.Count;
    public EnemyMaster this[int index] => rows[index];

    public IEnumerator<EnemyMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
