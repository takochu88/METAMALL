using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginBonusTable : ScriptableObject, IReadOnlyList<LoginBonusMaster>
{
    [SerializeField] private List<LoginBonusMaster> rows = new List<LoginBonusMaster>();

    public int Count => rows.Count;
    public LoginBonusMaster this[int index] => rows[index];

    public IEnumerator<LoginBonusMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
