using UnityEngine;

public class UseCase_TopMenu : MonoBehaviour
{
    private Main main;
    private TopMenuUI topMenuUI;
    
    public void Setup(Main main)
    {
        this.main = main;
        topMenuUI = main.ui.GetMenuUI(MenuType.Top) as TopMenuUI;
        topMenuUI.Setup(); 
        int menuIndex = (int)MenuType.Top;
        main.ui.selectMainMenuUI.selectTopMenuRowUIRight.SetupMain(menuIndex, main.useCase.handleMenu.OnSelectMainMenu);
        main.ui.selectMainMenuUI.selectTopMenuRowUILeft.SetupMain(menuIndex, main.useCase.handleMenu.OnSelectMainMenu);
    }

    public void OnRefresh()
    {
        main.useCase.inventory.OnRefresh();
        main.useCase.mail.OnRefresh();
    }
}
