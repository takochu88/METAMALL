using System.Collections.Generic;
using UnityEngine;

namespace MetaMall.Editor.MasterSheet
{
    [CreateAssetMenu(fileName = "MasterSheetConfig", menuName = "MetaMall/MasterSheetConfig")]
    public class MasterSheetConfig : ScriptableObject
    {
        public List<MasterSheetEntry> sheets = new List<MasterSheetEntry>();
    }
}
