public static partial class DebugCommandList
{
    static void RegisterCharacterCommands()
    {
        Add("全キャラアンロック", DebugCategory.Character, main =>
        {
            var charData = Mgr.Save.AllCharacterData;
            while (charData.lockedCharacters.Count > 0)
            {
                charData.Unlock(charData.lockedCharacters[0].masterId);
            }
            Mgr.Save.Save();
            Mgr.Toast.Show("全キャラをアンロックしました");
        });

        Add("全キャラロック", DebugCategory.Character, main =>
        {
            Mgr.Save.AllCharacterData.Init(Mgr.Master.characterTable);
            Mgr.Save.Save();
            Mgr.Toast.Show("全キャラをロックしました");
        });
    }
}
