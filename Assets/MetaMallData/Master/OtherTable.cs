using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherRewardTable : ScriptableObject, IReadOnlyList<OtherRewardMaster>
{
    [SerializeField] private List<OtherRewardMaster> rows = new List<OtherRewardMaster>();

    public int Count => rows.Count;
    public OtherRewardMaster this[int index] => rows[index];

    public IEnumerator<OtherRewardMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
