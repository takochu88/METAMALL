using UnityEngine;

public static partial class DebugCommandList
{
    static void RegisterOtherCommands()
    {
        Add("セーブデータリセット", DebugCategory.Other, main =>
        {
            PlayerPrefs.DeleteAll();
            Mgr.Instance.Setup(true);
            Mgr.Toast.Show("セーブデータをリセットしました");
        });
    }
}
