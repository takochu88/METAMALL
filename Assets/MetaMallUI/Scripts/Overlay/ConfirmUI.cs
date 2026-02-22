using System;
using TMPro;
using UnityEngine;

public class ConfirmUI : OverlayUIBase
{
    [SerializeField] private TMP_Text msgText;
    [SerializeField] private MyButton okButton;
    [SerializeField] private MyButton cancelButton;
    [SerializeField] private TMP_Text okText;
    [SerializeField] private TMP_Text cancelText;

    private Action onOk;
    private Action onCancel;

    private void Awake()
    {
        if (okButton != null) okButton.SetOnClick(OnOk);
        if (cancelButton != null) cancelButton.SetOnClick(OnCancel);
    }

    public void SetData(string title, string msg, Action onOk, Action onCancel = null,  string okText = null, string cancelText = null)
    {
        this.onOk = onOk;
        this.onCancel = onCancel;
        titleText.SetText(title);
        msgText.SetText(msg);
        this.okText.text = okText == null ? Mgr.Local.Get("ui-ok") : okText;
        this.cancelText.text = cancelText == null ? Mgr.Local.Get("ui-cancel") : cancelText;
    }

    private void OnOk()
    {
        onOk?.Invoke();
        Hide();
    }

    private void OnCancel()
    {
        onCancel?.Invoke();
        Hide();
    }
}
