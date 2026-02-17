using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentTable : ScriptableObject, IReadOnlyList<EquipmentMaster>
{
    [SerializeField] private List<EquipmentMaster> rows = new List<EquipmentMaster>();

    public int Count => rows.Count;
    public EquipmentMaster this[int index] => rows[index];

    public IEnumerator<EquipmentMaster> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
