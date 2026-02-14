using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MetaMall.Editor.MasterSheet
{
    public class MasterSheetWindow : EditorWindow
    {
        private MasterSheetConfig _config;
        private Vector2 _scrollPos;
        private string _searchText = "";

        [MenuItem("MetaMall/Master Sheet")]
        private static void Open()
        {
            GetWindow<MasterSheetWindow>("Master Sheet");
        }

        private void OnEnable()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            var guids = AssetDatabase.FindAssets("t:MasterSheetConfig");
            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                _config = AssetDatabase.LoadAssetAtPath<MasterSheetConfig>(path);
            }
        }

        private void OnGUI()
        {
            if (_config == null)
            {
                EditorGUILayout.HelpBox(
                    "MasterSheetConfig が見つかりません。\nCreate > MetaMall > MasterSheetConfig で作成してください。",
                    MessageType.Warning);
                if (GUILayout.Button("再検索"))
                    LoadConfig();
                return;
            }

            DrawHeader();
            DrawSearchBar();

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            DrawEntries();
            EditorGUILayout.EndScrollView();
        }

        // ── Header ──────────────────────────────────────────

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Master Sheet", EditorStyles.boldLabel);

            if (GUILayout.Button("+ Create", GUILayout.Width(80)))
                OnCreate();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(2);
        }

        // ── Search ──────────────────────────────────────────

        private void DrawSearchBar()
        {
            _searchText = EditorGUILayout.TextField(_searchText, EditorStyles.toolbarSearchField);
            EditorGUILayout.Space(4);
        }

        // ── Entry List ──────────────────────────────────────

        private void DrawEntries()
        {
            MasterSheetEntry toDelete = null;

            foreach (var entry in _config.sheets)
            {
                if (!MatchesSearch(entry)) continue;
                if (DrawEntryRow(entry))
                    toDelete = entry;
            }

            if (toDelete != null)
            {
                _config.sheets.Remove(toDelete);
                EditorUtility.SetDirty(_config);
                AssetDatabase.SaveAssets();
            }
        }

        private bool MatchesSearch(MasterSheetEntry entry)
        {
            if (string.IsNullOrEmpty(_searchText)) return true;
            return entry.SheetName.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        // ── Entry Row ───────────────────────────────────────

        private bool DrawEntryRow(MasterSheetEntry entry)
        {
            EditorGUILayout.BeginHorizontal();

            // 名前クリック → Table 選択
            var hasTable = entry.outputTable != null;
            var nameStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = hasTable ? FontStyle.Bold : FontStyle.Italic,
                normal =
                {
                    textColor = hasTable
                        ? (EditorGUIUtility.isProSkin
                            ? new Color(0.6f, 0.85f, 1f)
                            : new Color(0.1f, 0.3f, 0.7f))
                        : Color.gray
                },
            };

            var nameContent = new GUIContent(entry.SheetName);
            var nameRect = GUILayoutUtility.GetRect(nameContent, nameStyle, GUILayout.MinWidth(120));
            EditorGUIUtility.AddCursorRect(nameRect, MouseCursor.Link);

            if (GUI.Button(nameRect, nameContent, nameStyle))
            {
                if (hasTable)
                {
                    Selection.activeObject = entry.outputTable;
                    EditorGUIUtility.PingObject(entry.outputTable);
                }
                else
                {
                    Selection.activeObject = _config;
                }
            }

            GUILayout.FlexibleSpace();

            // Open
            GUI.enabled = !string.IsNullOrEmpty(entry.spreadsheetId);
            if (GUILayout.Button("Open", GUILayout.Width(50)))
            {
                var gid = string.IsNullOrEmpty(entry.gid) ? "" : $"#gid={entry.gid}";
                Application.OpenURL(
                    $"https://docs.google.com/spreadsheets/d/{entry.spreadsheetId}{gid}");
            }
            GUI.enabled = true;

            // Import
            GUI.enabled = hasTable && !string.IsNullOrEmpty(entry.spreadsheetId);
            if (GUILayout.Button("Import", GUILayout.Width(55)))
            {
                CsvImporter.Import(entry);
            }
            GUI.enabled = true;

            // 削除
            bool deleteRequested = false;
            if (GUILayout.Button("x", GUILayout.Width(22)))
            {
                deleteRequested = EditorUtility.DisplayDialog(
                    "削除確認", $"{entry.SheetName} を一覧から削除しますか？", "削除", "キャンセル");
            }

            EditorGUILayout.EndHorizontal();
            return deleteRequested;
        }

        // ── + Create ────────────────────────────────────────

        private void OnCreate()
        {
            var tableTypes = FindAllTableTypes();
            var dir = "Assets/MetaMallData/Master";

            if (!AssetDatabase.IsValidFolder(dir))
                AssetDatabase.CreateFolder("Assets/MetaMallData", "Master");

            // まず未登録の型を自動生成
            bool anyAutoCreated = false;
            foreach (var type in tableTypes)
            {
                bool alreadyExists = _config.sheets.Any(s =>
                    s.outputTable != null && s.outputTable.GetType() == type);
                if (alreadyExists) continue;

                var table = FindOrCreateAsset(type, dir);
                _config.sheets.Add(new MasterSheetEntry
                {
                    outputTable = table,
                    gid = "0",
                });
                anyAutoCreated = true;
            }

            if (anyAutoCreated)
            {
                EditorUtility.SetDirty(_config);
                AssetDatabase.SaveAssets();
                return;
            }

            // 全型が登録済み → 追加インスタンスを作れるドロップダウンを表示
            ShowAddInstanceMenu(tableTypes, dir);
        }

        private void ShowAddInstanceMenu(List<Type> tableTypes, string dir)
        {
            var menu = new GenericMenu();

            foreach (var type in tableTypes)
            {
                var t = type;
                menu.AddItem(new GUIContent(t.Name), false, () =>
                {
                    var table = CreateUniqueAsset(t, dir);
                    _config.sheets.Add(new MasterSheetEntry
                    {
                        outputTable = table,
                        gid = "0",
                    });
                    EditorUtility.SetDirty(_config);
                    AssetDatabase.SaveAssets();
                    EditorGUIUtility.PingObject(table);
                });
            }

            menu.ShowAsContext();
        }

        // ── Asset Helpers ───────────────────────────────────

        private static ScriptableObject FindOrCreateAsset(Type tableType, string dir)
        {
            var path = $"{dir}/{tableType.Name}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (existing != null) return existing;

            var instance = CreateInstance(tableType);
            AssetDatabase.CreateAsset(instance, path);
            return instance;
        }

        /// <summary>重複しない名前で新しいアセットを生成する。</summary>
        private static ScriptableObject CreateUniqueAsset(Type tableType, string dir)
        {
            var baseName = tableType.Name;
            var path = AssetDatabase.GenerateUniqueAssetPath($"{dir}/{baseName}.asset");

            var instance = CreateInstance(tableType);
            AssetDatabase.CreateAsset(instance, path);
            return instance;
        }

        // ── Utility ─────────────────────────────────────────

        private static List<Type> FindAllTableTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return Type.EmptyTypes; }
                })
                .Where(t => t.IsSubclassOf(typeof(ScriptableObject))
                         && !t.IsAbstract
                         && t.GetField("rows", BindingFlags.NonPublic | BindingFlags.Instance) != null)
                .OrderBy(t => t.Name)
                .ToList();
        }
    }
}
