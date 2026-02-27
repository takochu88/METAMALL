using System;
using UnityEngine;

public class CharacterListUI : MonoBehaviour
{
    [SerializeField] private MyScrollRect scroll;

    private Action<int> onSelectCharacter;

    private int SelectedIndex
    {
        get => Mgr.Save.SessionData.GetIndex(MenuType.CustomCharacter, -1);
        set => Mgr.Save.SessionData.SetIndex(MenuType.CustomCharacter, value);
    }

    public void Setup(Action<int> onSelectCharacter)
    {
        this.onSelectCharacter = onSelectCharacter;
    }

    public void Refresh()
    {
        var characters = Mgr.Save.AllCharacterData.unlockedCharacters;

        // デフォルト選択: 未選択なら先頭キャラを選択
        if (SelectedIndex < 0 && characters.Count > 0)
        {
            SelectedIndex = characters[0].Master.index;
            onSelectCharacter?.Invoke(SelectedIndex);
        }

        scroll.Init(
            characters.Count,
            cell => cell.Get<CharacterCellUI>().Init(OnSelect),
            (index, cell) =>
            {
                var row = (CharacterCellUI)cell;
                row.SetData(characters[index]);
                row.SetSelected(characters[index].Master.index == SelectedIndex);
            }
        );
    }

    public void Select(int masterIndex)
    {
        SelectedIndex = masterIndex;
        onSelectCharacter?.Invoke(masterIndex);
        Refresh();
    }

    private void OnSelect(int masterIndex)
    {
        Select(masterIndex);
    }
}
