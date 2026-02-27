using UnityEngine;

public class UseCase_MetaTask : MonoBehaviour
{
    private Main main;

    public void Setup(Main main)
    {
        this.main = main;
        int menuIndex = (int)MenuType.Arcade;
        main.ui.selectMainMenuUI.selectArcadeMenuRowUI.SetupMain(menuIndex, main.useCase.handleMenu.OnSelectMainMenu);
    }
}
