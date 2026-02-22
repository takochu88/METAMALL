#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MyScrollRect))]
public class MyScrollRectEditor : OdinEditor
{
    private static readonly string[] hiddenByRewardList =
    {
        "cellSizeModeY", "cellHeight",
        "paddingTop", "paddingBottom",
        "spacing",
    };

    private bool isRewardListChild;

    protected override void OnEnable()
    {
        base.OnEnable();
        var t = target as MyScrollRect;
        isRewardListChild = t != null && t.GetComponentInParent<RewardListUI>() != null;
    }

    protected override void DrawTree()
    {
        if (isRewardListChild)
        {
            foreach (var prop in Tree.EnumerateTree(false))
            {
                if (System.Array.IndexOf(hiddenByRewardList, prop.Name) >= 0)
                    prop.State.Visible = false;
            }
        }

        base.DrawTree();
    }
}
#endif
