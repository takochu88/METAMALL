using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace MetaMall.Editor.MasterSheet
{
    public static class CsvImporter
    {
        /// <summary>
        /// GoogleスプレッドシートからCSVをダウンロードし、ScriptableObjectのrowsフィールドにデータを書き込む。
        /// </summary>
        public static void Import(MasterSheetEntry entry, Action onComplete = null)
        {
            if (entry.outputTable == null)
            {
                Debug.LogError($"[CsvImporter] outputTable が設定されていません: {entry.SheetName}");
                return;
            }

            var url = $"https://docs.google.com/spreadsheets/d/{entry.spreadsheetId}/export?format=csv&gid={entry.gid}";
            var request = UnityWebRequest.Get(url);

            EditorUtility.DisplayProgressBar("MasterSheet Import", $"{entry.SheetName} をダウンロード中...", 0.2f);

            var operation = request.SendWebRequest();
            operation.completed += _ =>
            {
                try
                {
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError($"[CsvImporter] ダウンロード失敗: {request.error}");
                        return;
                    }

                    EditorUtility.DisplayProgressBar("MasterSheet Import", $"{entry.SheetName} をパース中...", 0.6f);

                    var csv = request.downloadHandler.text;
                    ApplyCsvToTable(csv, entry.outputTable);

                    EditorUtility.SetDirty(entry.outputTable);
                    AssetDatabase.SaveAssets();

                    Debug.Log($"[CsvImporter] {entry.SheetName} のインポートが完了しました。");
                    onComplete?.Invoke();
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                    request.Dispose();
                }
            };
        }

        /// <summary>
        /// CSVテキストをパースし、ScriptableObjectのrowsフィールドにリフレクションで書き込む。
        /// </summary>
        private static void ApplyCsvToTable(string csv, ScriptableObject table)
        {
            var lines = ParseCsvLines(csv);
            if (lines.Count < 3)
            {
                Debug.LogWarning("[CsvImporter] データ行がありません（3行未満）。");
                return;
            }

            var headers = lines[0];   // フィールド名
            var typeHints = lines[1];  // 型ヒント（string, int, float）
            var dataLines = lines.GetRange(2, lines.Count - 2);

            // rowsフィールドを取得
            var rowsField = table.GetType().GetField("rows", BindingFlags.NonPublic | BindingFlags.Instance);
            if (rowsField == null)
            {
                Debug.LogError($"[CsvImporter] {table.GetType().Name} に 'rows' フィールドが見つかりません。");
                return;
            }

            // rowsのジェネリック型引数からMasterクラスの型を取得
            var listType = rowsField.FieldType;
            var elementType = listType.GetGenericArguments()[0];

            // IList として操作
            var list = (IList)Activator.CreateInstance(listType);

            foreach (var dataRow in dataLines)
            {
                var instance = Activator.CreateInstance(elementType);

                for (int col = 0; col < headers.Count; col++)
                {
                    var fieldName = headers[col].Trim();
                    if (string.IsNullOrEmpty(fieldName)) continue;

                    var field = elementType.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
                    if (field == null) continue;

                    var rawValue = col < dataRow.Count ? dataRow[col].Trim() : "";
                    var typeHint = col < typeHints.Count ? typeHints[col].Trim().ToLower() : "string";

                    var converted = ConvertValue(rawValue, typeHint, field.FieldType);
                    if (converted != null)
                    {
                        field.SetValue(instance, converted);
                    }
                }

                list.Add(instance);
            }

            rowsField.SetValue(table, list);
        }

        /// <summary>
        /// 型ヒントに基づいて文字列を適切な型に変換する。
        /// </summary>
        private static object ConvertValue(string raw, string typeHint, Type targetType)
        {
            if (string.IsNullOrEmpty(raw))
            {
                if (targetType == typeof(string)) return "";
                if (targetType == typeof(int)) return 0;
                if (targetType == typeof(float)) return 0f;
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            }

            switch (typeHint)
            {
                case "int":
                    return int.TryParse(raw, out var i) ? i : 0;
                case "float":
                    return float.TryParse(raw, out var f) ? f : 0f;
                case "string":
                default:
                    return raw;
            }
        }

        /// <summary>
        /// CSV文字列を行×列のリストにパースする。ダブルクォート対応。
        /// </summary>
        private static List<List<string>> ParseCsvLines(string csv)
        {
            var result = new List<List<string>>();
            var row = new List<string>();
            var cell = "";
            var inQuotes = false;

            for (int i = 0; i < csv.Length; i++)
            {
                var c = csv[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < csv.Length && csv[i + 1] == '"')
                        {
                            cell += '"';
                            i++;
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        cell += c;
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        inQuotes = true;
                    }
                    else if (c == ',')
                    {
                        row.Add(cell);
                        cell = "";
                    }
                    else if (c == '\r')
                    {
                        // skip
                    }
                    else if (c == '\n')
                    {
                        row.Add(cell);
                        cell = "";
                        result.Add(row);
                        row = new List<string>();
                    }
                    else
                    {
                        cell += c;
                    }
                }
            }

            // 最終行
            if (cell.Length > 0 || row.Count > 0)
            {
                row.Add(cell);
                result.Add(row);
            }

            return result;
        }
    }
}
