using UnityEngine;

public class UseCase_GameEvent : MonoBehaviour
{
    private Main main;

    public void Setup(Main main)
    {
        this.main = main;
        int menuIndex = (int)MenuType.GameEvent;
        main.ui.selectMainMenuUI.selectGameEventMenuRowUI.SetupMain(menuIndex, main.useCase.handleMenu.OnSelectMainMenu);
    }
}
