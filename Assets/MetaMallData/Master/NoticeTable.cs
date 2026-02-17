using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeTable : ScriptableObject, IReadOnlyList<NoticeMaster>
{
    [SerializeField] private List<NoticeMaster> rows = new List<NoticeMaster>();

    public int Count => rows.Count;
    public NoticeMaster this[int index] => rows[index];

    public IEnumerator<NoticeMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
