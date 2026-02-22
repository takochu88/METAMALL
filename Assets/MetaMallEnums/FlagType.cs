public enum FlagType
{
    None = -1,
    FirstLaunch = 0,
    
    //メインメニューのアンロックフラグ 100~
    Unlock_TopMenu = 100,
    Unlock_StageMenu = 101,
    Unlock_GameEventMenu = 102,
    Unlock_PurchaseMenu = 103,
    Unlock_GachaMenu = 104,
    
    //サブメニューのアンロックフラグ 200~
    Unlock_MailMenu = 200,
    Unlock_NoticeMenu　 = 201,
    Unlock_ConfigMenu　 = 202,
    Unlock_InventoryMenu　 = 203,
    Unlock_CustomCharacter = 204,
    Unlock_Formation = 205,
}
