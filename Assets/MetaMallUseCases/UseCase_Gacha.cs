using UnityEngine;

public class UseCase_Gacha : MonoBehaviour
{
    private Main main;

    public void Setup(Main main)
    {
        this.main = main;
        int menuIndex = (int)MenuType.Gacha;
        main.ui.selectMainMenuUI.selectGachaMenuRowUI.SetupMain(menuIndex, main.useCase.handleMenu.OnSelectMainMenu);
    }
}
