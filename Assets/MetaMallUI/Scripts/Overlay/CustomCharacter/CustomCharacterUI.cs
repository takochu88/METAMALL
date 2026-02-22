using UnityEngine;
using UnityEngine.UI;

public class CustomCharacterUI : OverlayUIBase
{
    [SerializeField] private Image fullBodyImage;
    [SerializeField] private CharacterListUI characterListUI;
    [SerializeField] private TalentUI talentUI;

    public void Setup()
    {
        characterListUI.Setup(OnSelectCharacter);
        talentUI.Setup();
    }

    public void Refresh()
    {
        characterListUI.Refresh();
    }

    public void UpdateUI(CharacterData data)
    {
        fullBodyImage.sprite = data.Master.FullBody;
        talentUI.UpdateUI(data);
    }

    private void OnSelectCharacter(int masterIndex)
    {
        var data = Mgr.Save.AllCharacterData.Get(masterIndex);
        if (data != null) UpdateUI(data);
    }
}
