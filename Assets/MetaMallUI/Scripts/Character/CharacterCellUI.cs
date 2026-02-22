using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCellUI : CellUIBase
{
    [SerializeField] private Image charaImage;

    private int index;
    private Action<int> onSelectCharacter;

    public void Init(Action<int> onSelectCharacter)
    {
        this.onSelectCharacter = onSelectCharacter;
        Button.SetOnClick(OnClick);
    }

    public void SetData(CharacterData data)
    {
        index = data.Master.index;
        charaImage.sprite = data.Master.FullBody;
    }

    private void OnClick()
    {
        onSelectCharacter?.Invoke(index);
    }
}
