using UnityEngine;

public class UseCase_CustomCharacter : MonoBehaviour
{
    private Main main;

    public void Setup(Main main)
    {
        this.main = main;
        CustomCharacterUI customCharacterUI = main.ui.OverlayStack.customCharacterUI;
        customCharacterUI.Setup();
        main.ui.TopMenuUI.characterMenuUI.selectCustomCharacterMenuUI.SetupSub(OnShowCustom);
    }

    public void OnShowCustom()
    {
        CustomCharacterUI customCharacterUI = main.ui.OverlayStack.customCharacterUI;
        customCharacterUI.Refresh();
        customCharacterUI.Show();
    }
}
