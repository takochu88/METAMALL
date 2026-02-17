using UnityEngine;

public class UseCase_Stage : MonoBehaviour
{
    private Main main;

    public void Setup(Main main)
    {
        this.main = main;

        var topUI = main.ui.GetMenuUI(MainMenuType.Top) as TopMenuUI;
        int menuIndex = (int) MainMenuType.Stage;
        main.ui.selectMainMenuUI.selectStageMenuRowUI.SetupMain(menuIndex, main.useCase.handleMenu.OnSelectMainMenu);
    }
}
