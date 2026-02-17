using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 単一選択のトグルグループ。子の MyToggle を自動検出して管理する。
/// </summary>
public class MyToggleGroup : MonoBehaviour
{
    // ── 設定 ─────────────────────────────────
    [BoxGroup("設定")]
    [Tooltip("初期選択インデックス")]
    [SerializeField] int defaultIndex;

    [BoxGroup("設定")]
    [Tooltip("初期選択をアニメなしで適用する")]
    [SerializeField] bool initInstant = true;

    // ── イベント ─────────────────────────────
    [BoxGroup("イベント")]
    [Tooltip("選択が変わった時に発火 (index)")]
    public UnityEvent<int> OnSelectionChanged;

    // ── 内部 ─────────────────────────────────
    readonly List<MyToggle> toggles = new();
    int currentIndex = -1;

    // ── プロパティ ───────────────────────────
    public int CurrentIndex => currentIndex;
    public int Count => toggles.Count;

    // ── ライフサイクル ──────────────────────
    void Awake()
    {
        CollectToggles();
    }

    void Start()
    {
        if (toggles.Count > 0)
            Select(Mathf.Clamp(defaultIndex, 0, toggles.Count - 1), initInstant);
    }

    // ── Public API ──────────────────────────

    /// <summary>指定インデックスを選択する。</summary>
    public void Select(int index, bool instant = false)
    {
        index = Mathf.Clamp(index, 0, toggles.Count - 1);
        if (index == currentIndex) return;

        // 前の選択を解除
        if (currentIndex >= 0 && currentIndex < toggles.Count)
            toggles[currentIndex].SetSelected(false, instant);

        currentIndex = index;
        toggles[currentIndex].SetSelected(true, instant);

        OnSelectionChanged?.Invoke(currentIndex);
    }

    /// <summary>トグルを取得する。</summary>
    public MyToggle GetToggle(int index) => toggles[index];

    /// <summary>子トグルを再収集する。動的に追加した場合に呼ぶ。</summary>
    public void Refresh()
    {
        CollectToggles();
    }

    // ── 内部 ─────────────────────────────────
    void CollectToggles()
    {
        toggles.Clear();
        GetComponentsInChildren(true, toggles);

        for (int i = 0; i < toggles.Count; i++)
            toggles[i].Init(this, i);
    }

#if UNITY_EDITOR
    [BoxGroup("デバッグ")]
    [SerializeField, PropertyOrder(100)]
    int debugSelectIndex;

    [BoxGroup("デバッグ"), ButtonGroup("デバッグ/btn"), PropertyOrder(101)]
    [Button("選択 (アニメ)")]
    void DebugSelect() => Select(debugSelectIndex);

    [BoxGroup("デバッグ"), ButtonGroup("デバッグ/btn"), PropertyOrder(101)]
    [Button("選択 (即座)")]
    void DebugSelectInstant() => Select(debugSelectIndex, true);
#endif
}
