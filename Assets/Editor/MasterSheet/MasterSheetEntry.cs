using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace MetaMall.Editor.MasterSheet
{
    [Serializable]
    public class MasterSheetEntry
    {
        [FormerlySerializedAs("category")] public MasterSheetCategory sheetCategory;
        public string spreadsheetId;
        public string gid = "0";
        public ScriptableObject outputTable;

        /// <summary>outputTable のアセット名から自動取得。</summary>
        public string SheetName => outputTable != null ? outputTable.name : "(未設定)";
    }
}
