using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventTable : ScriptableObject, IReadOnlyList<GameEventMaster>
{
    [SerializeField] private List<GameEventMaster> rows = new List<GameEventMaster>();

    public int Count => rows.Count;
    public GameEventMaster this[int index] => rows[index];

    public IEnumerator<GameEventMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
