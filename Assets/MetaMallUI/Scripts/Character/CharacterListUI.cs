using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterListUI : MonoBehaviour
{
    [SerializeField] private MyScrollRect scroll;

    private Action<int> onSelectCharacter;

    public void Setup(Action<int> onSelectCharacter)
    {
        this.onSelectCharacter = onSelectCharacter;
    }

    public void Refresh()
    {
        var characters = Mgr.Save.AllCharacterData.unlockedCharacters;
        scroll.Init(
            characters.Count,
            cell => cell.Get<CharacterListRowUI>().Init(onSelectCharacter),
            (index, cell) => cell.Get<CharacterListRowUI>().Setup(characters[index])
        );
    }
}
