using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockCharacterTable : ScriptableObject, IReadOnlyList<UnlockCharacterMaster>
{
    [SerializeField] private List<UnlockCharacterMaster> rows = new List<UnlockCharacterMaster>();

    public int Count => rows.Count;
    public UnlockCharacterMaster this[int index] => rows[index];

    public IEnumerator<UnlockCharacterMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
