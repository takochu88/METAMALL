using UnityEngine;

public class UseCase_Purchase : MonoBehaviour
{
    private Main main;

    public void Setup(Main main)
    {
        this.main = main;
        int menuIndex = (int)MenuType.Purchase;
        main.ui.selectMainMenuUI.selectPurchaseMenuRowUI.SetupMain(menuIndex, main.useCase.handleMenu.OnSelectMainMenu);
    }
}
