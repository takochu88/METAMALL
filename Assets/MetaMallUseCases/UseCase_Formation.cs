using UnityEngine;

public class UseCase_Formation : MonoBehaviour
{
    private Main main;

    public void Setup(Main main)
    {
        this.main = main;
        main.ui.OverlayStack.formationUI.Setup();
        main.ui.TopMenuUI.characterMenuUI.selectFormationMenuUI.SetupSub(OnShowFormation);
    }

    public void OnShowFormation()
    {
        main.ui.OverlayStack.formationUI.Show();
    }
}
