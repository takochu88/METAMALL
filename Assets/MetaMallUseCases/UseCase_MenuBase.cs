using UnityEngine;

/// <summary>
/// セル選択インデックスを SessionData に保存する UseCase の基底クラス。
/// </summary>
public abstract class UseCase_MenuBase : MonoBehaviour
{
    protected abstract MenuType MenuType { get; }

    protected Main main;

    protected int SelectedIndex
    {
        get => Mgr.Save.SessionData.GetIndex(MenuType);
        set => Mgr.Save.SessionData.SetIndex(MenuType, value);
    }

    protected virtual void OnSelectCell(int index)
    {
        SelectedIndex = index;
    }
}
