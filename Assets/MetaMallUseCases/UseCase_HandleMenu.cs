using PrimeTween;
using UnityEngine;

public class UseCase_HandleMenu : MonoBehaviour
{
    private Main main;
    private MainMenuType currentMainMenuType;

    [Header("スライド設定")]
    [SerializeField] float slideDuration = 0.3f;
    [SerializeField] Ease slideEase = Ease.OutCubic;

    public MainMenuType CurrentMainMenuType => currentMainMenuType;

    public void Setup(Main main)
    {
        this.main = main;
    }

    public void OnSelectMainMenu(int menuTypeIndex)
    {
        MainMenuType newMainMenuType = (MainMenuType)menuTypeIndex;
        if (currentMainMenuType == newMainMenuType) return;

        MainMenuType prevMainMenuType = currentMainMenuType;
        currentMainMenuType = newMainMenuType;

        MenuUIBase prevMenuUI = main.ui.GetMenuUI(prevMainMenuType);
        MenuUIBase newMenuUI  = main.ui.GetMenuUI(newMainMenuType);

        // 同じUIなら再表示だけ
        if (prevMenuUI == newMenuUI)
        {
            newMenuUI.ShowImmediate();
            return;
        }

        MainMenuUIPosType prevPosType = Mgr.Master.mainMenuTable[(int)prevMainMenuType].posType;
        MainMenuUIPosType newPosType  = Mgr.Master.mainMenuTable[menuTypeIndex].posType;
        
        // ── スライド方向を決定 ──
        // posType が違う場合: 位置関係で判定（Left=0 < Center=1 < Right=2）
        // posType が同じ場合: enum の並び順で判定
        bool moveRight;
        if (prevPosType != newPosType)
            moveRight = (int)newPosType > (int)prevPosType;
        else
            moveRight = (int)newMainMenuType > (int)prevMainMenuType;

        // UI操作をブロック＋SelectMenuを一時的に隠す
        main.ui.SetInteractable(false);
        main.ui.selectMainMenuUI.HideImmediate();

        // 前のメニューをスライドアウト（新しい方向の逆へ退場）
        if (prevMenuUI != null)
            prevMenuUI.SlideOut(moveRight, slideDuration, slideEase);

        // 新しいメニューをスライドイン（完了時にSelectMenu表示＋UI操作復帰）
        if (newMenuUI != null)
            newMenuUI.SlideIn(moveRight, slideDuration, slideEase, OnSlideComplete);
    }

    private void OnSlideComplete()
    {
        SelectMainMenuUI selectMainMenuUI = main.ui.selectMainMenuUI;
        
        switch (currentMainMenuType)
        {
            case MainMenuType.Top:
                selectMainMenuUI.selectTopMenuRowUIRight.SetVisible(false);
                selectMainMenuUI.selectTopMenuRowUILeft.SetVisible(false);
                selectMainMenuUI.selectStageMenuRowUI.SetVisible(true);
                selectMainMenuUI.selectGameEventMenuRowUI.SetVisible(true);
                selectMainMenuUI.selectPurchaseMenuRowUI.SetVisible(true);
                selectMainMenuUI.selectGachaMenuRowUI.SetVisible(true);
                break;
            
            case MainMenuType.Stage:
                selectMainMenuUI.selectTopMenuRowUIRight.SetVisible(false);
                selectMainMenuUI.selectTopMenuRowUILeft.SetVisible(true);
                selectMainMenuUI.selectStageMenuRowUI.SetVisible(false);
                selectMainMenuUI.selectGameEventMenuRowUI.SetVisible(false);
                selectMainMenuUI.selectPurchaseMenuRowUI.SetVisible(false);
                selectMainMenuUI.selectGachaMenuRowUI.SetVisible(false);
                break;
            
            case MainMenuType.GameEvent:
                selectMainMenuUI.selectTopMenuRowUIRight.SetVisible(false);
                selectMainMenuUI.selectTopMenuRowUILeft.SetVisible(true);
                selectMainMenuUI.selectStageMenuRowUI.SetVisible(false);
                selectMainMenuUI.selectGameEventMenuRowUI.SetVisible(false);
                selectMainMenuUI.selectPurchaseMenuRowUI.SetVisible(false);
                selectMainMenuUI.selectGachaMenuRowUI.SetVisible(false);
                break;
            
            case MainMenuType.Purchase:
                selectMainMenuUI.selectTopMenuRowUIRight.SetVisible(true);
                selectMainMenuUI.selectTopMenuRowUILeft.SetVisible(false);
                selectMainMenuUI.selectStageMenuRowUI.SetVisible(false);
                selectMainMenuUI.selectGameEventMenuRowUI.SetVisible(false);
                selectMainMenuUI.selectPurchaseMenuRowUI.SetVisible(false);
                selectMainMenuUI.selectGachaMenuRowUI.SetVisible(false);
                break;
            
            case MainMenuType.Gacha:
                selectMainMenuUI.selectTopMenuRowUIRight.SetVisible(true);
                selectMainMenuUI.selectTopMenuRowUILeft.SetVisible(false);
                selectMainMenuUI.selectStageMenuRowUI.SetVisible(false);
                selectMainMenuUI.selectGameEventMenuRowUI.SetVisible(false);
                selectMainMenuUI.selectPurchaseMenuRowUI.SetVisible(false);
                selectMainMenuUI.selectGachaMenuRowUI.SetVisible(false);
                break;
                
        }
        
        selectMainMenuUI.Show();
        main.ui.SetInteractable(true);
        main.ui.footerMenuUI.SetMenuTitle("バトル");
    }
}
