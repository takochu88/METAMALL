using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterListRowUI : MonoBehaviour
{
    [SerializeField] private MyButton button;
    [SerializeField] private Image charaImage;

    private int index;
    private Action<int> onSelectCharacter;

    public void Init(Action<int> onSelectCharacter)
    {
        this.onSelectCharacter = onSelectCharacter;
        button.SetOnClick(OnClick);
    }

    public void Setup(CharacterData data)
    {
        index = data.Master.index;
        //charaImage.sprite = charaSprite;
    }

    private void OnClick()
    {
        onSelectCharacter?.Invoke(index);
    }
}
