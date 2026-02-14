using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTable : ScriptableObject, IReadOnlyList<CharacterMaster>
{
    [SerializeField] private List<CharacterMaster> rows = new List<CharacterMaster>();

    public int Count => rows.Count;
    public CharacterMaster this[int index] => rows[index];

    public IEnumerator<CharacterMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
