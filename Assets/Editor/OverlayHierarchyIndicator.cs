using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 再生中のヒエラルキーで OverlayUIBase の表示状態を色で示す。
/// ● 緑 = Shown / ● 灰 = Hidden
/// 状態が変わった時だけ再描画する。
/// </summary>
[InitializeOnLoad]
static class OverlayHierarchyIndicator
{
    static readonly Color shownColor = new Color(0.3f, 1f, 0.3f);
    static readonly Color hiddenColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    static readonly Dictionary<int, OverlayUIBase> cache = new();
    static readonly Dictionary<int, bool> prevStates = new();

    static OverlayHierarchyIndicator()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        EditorApplication.playModeStateChanged += _ => { cache.Clear(); prevStates.Clear(); };
        EditorApplication.update += OnUpdate;
    }

    static void OnUpdate()
    {
        if (!Application.isPlaying) return;

        bool dirty = false;
        foreach (var kvp in cache)
        {
            if (kvp.Value == null) continue;
            bool shown = kvp.Value.IsShown;
            if (!prevStates.TryGetValue(kvp.Key, out var prev) || prev != shown)
            {
                prevStates[kvp.Key] = shown;
                dirty = true;
            }
        }

        if (dirty)
            EditorApplication.RepaintHierarchyWindow();
    }

    static void OnHierarchyGUI(int instanceID, Rect rect)
    {
        if (!Application.isPlaying) return;

        if (!cache.TryGetValue(instanceID, out var overlay))
        {
            var go = EditorUtility.EntityIdToObject(instanceID) as GameObject;
            overlay = go != null ? go.GetComponent<OverlayUIBase>() : null;
            cache[instanceID] = overlay;
        }

        if (overlay == null) return;

        var iconRect = new Rect(rect.xMax - 16, rect.y + 2, 12, 12);
        var color = overlay.IsShown ? shownColor : hiddenColor;

        var prev = GUI.color;
        GUI.color = color;
        GUI.Label(iconRect, "●");
        GUI.color = prev;
    }
}
