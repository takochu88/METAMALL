using System;
using TMPro;
using UnityEngine;

public class MailCellUI : CellUIBase
{
    [SerializeField] private TMP_Text dateLabel;

    private Action<int> onClick;
    private int index;

    public void Init(Action<int> onClick)
    {
        this.onClick = onClick;
        Button.SetOnClick(OnClick);
    }

    public void SetData(int index, MailItem mail, bool claimed)
    {
        this.index = index;
        Title.text = mail.Title;
        dateLabel.text = mail.Timestamp.ToString("yyyy/MM/dd");
        SetBadgeVisible(mail.HasRewards && !claimed);
    }

    private void OnClick()
    {
        onClick?.Invoke(index);
    }
}
