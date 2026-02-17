using System;
using TMPro;
using UnityEngine;

public class DebugCommandRowUI : MonoBehaviour
{
    [SerializeField] MyButton button;
    [SerializeField] TMP_Text nameLabel;
    [SerializeField] TMP_Text categoryLabel;

    Action onClickCallback;

    public void Init(Action<DebugCommand> onSelect)
    {
        button.SetOnClick(() => onClickCallback?.Invoke());
    }

    public void Setup(DebugCommand command, Action<DebugCommand> onSelect)
    {
        nameLabel.text = command.Name;
        categoryLabel.text = command.Category.ToString();
        onClickCallback = () => onSelect?.Invoke(command);
    }
}
