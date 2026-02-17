using UnityEditor;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public static class ClaudeBridgeServer
{
    static string commandPath = "ClaudeCommand.json";

    static ClaudeBridgeServer()
    {
        EditorApplication.update += Update;
    }

    static void Update()
    {
        if (!File.Exists(commandPath)) return;

        string json = File.ReadAllText(commandPath);
        File.Delete(commandPath);

        Execute(json);
    }

    static void Execute(string json)
    {
        Debug.Log("Claude Command: " + json);

        // 例：Prefab開いて保存
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Test.prefab");

        if (prefab != null)
        {
            EditorUtility.SetDirty(prefab);
            AssetDatabase.SaveAssets();
            Debug.Log("Prefab saved by Claude");
        }
    }
}