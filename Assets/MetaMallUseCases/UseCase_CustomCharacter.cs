using UnityEngine;

public class UseCase_CustomCharacter : MonoBehaviour
{
    private Main main;

    public void Setup(Main main)
    {
        this.main = main;
        CustomCharacterUI customCharacterUI = main.ui.OverlayStack.customCharacterUI;
        customCharacterUI.Setup();
        customCharacterUI.characterListUI.Setup(OnSelectCharacter);
        main.ui.TopMenuUI.characterMenuUI.selectCustomCharacterMenuUI.SetupSub(OnShowCustom);
    }

    public void OnShowCustom()
    {
        CustomCharacterUI customCharacterUI = main.ui.OverlayStack.customCharacterUI;
        customCharacterUI.characterListUI.Refresh();
        customCharacterUI.Show();
    }

    private void OnSelectCharacter(int index)
    {
        Debug.Log(index);
    }
}
