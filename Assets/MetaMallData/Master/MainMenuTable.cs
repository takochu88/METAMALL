using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuTable : ScriptableObject, IReadOnlyList<MainMenuMaster>
{
    [SerializeField] private List<MainMenuMaster> rows = new List<MainMenuMaster>();

    public int Count => rows.Count;
    public MainMenuMaster this[int index] => rows[index];

    public IEnumerator<MainMenuMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
