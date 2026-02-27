using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StabilityDetailUI : OverlayUIBase
{
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private TMP_Text nextTimerText;
    [SerializeField] private TMP_Text fullTimerText;
    [SerializeField] private Image fillImage;
    [SerializeField] private MyButton recoveryButton;

    private int displayedAmount = -1;
    private Action onRecovery;

    public void Setup(Action onRecovery)
    {
        this.onRecovery = onRecovery;
        if (recoveryButton != null) recoveryButton.SetOnClick(OnRecoveryClicked);
    }

    private void OnEnable()
    {
        Mgr.Save.StabilityData.OnChanged += Refresh;
    }

    private void OnDisable()
    {
        Mgr.Save.StabilityData.OnChanged -= Refresh;
    }

    private void Update()
    {
        if (!IsShown) return;

        var data = Mgr.Save.StabilityData;
        if (data.IsFull) return;

        int current = data.Amount;
        if (current != displayedAmount)
            UpdateAmount(data, current);

        UpdateTimers();
    }

    private void Refresh()
    {
        var data = Mgr.Save.StabilityData;
        UpdateAmount(data, data.Amount);
        UpdateTimers();
        UpdateRecoveryButton();
    }

    private void UpdateAmount(StabilityData data, int current)
    {
        displayedAmount = current;
        amountText.text = $"{current}/{data.Max}";
        if (fillImage != null)
            fillImage.fillAmount = data.Max > 0 ? (float)current / data.Max : 0f;
    }

    private void UpdateTimers()
    {
        var data = Mgr.Save.StabilityData;
        bool showTimer = !data.IsFull;

        nextTimerText.gameObject.SetActiveIfChanged(showTimer);
        fullTimerText.gameObject.SetActiveIfChanged(showTimer);
        if (!showTimer) return;

        var toNext = data.GetTimeToNext();
        nextTimerText.text = $"{(int)toNext.TotalMinutes:D2}:{toNext.Seconds:D2}";

        var toFull = data.GetTimeToFull();
        fullTimerText.text = FormatFullTimer(toFull);
    }

    private void UpdateRecoveryButton()
    {
        if (recoveryButton != null)
            recoveryButton.gameObject.SetActiveIfChanged(!Mgr.Save.StabilityData.IsFull);
    }

    private static string FormatFullTimer(TimeSpan ts)
    {
        if (ts.TotalHours >= 1)
            return $"{(int)ts.TotalHours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}";
        return $"{ts.Minutes:D2}:{ts.Seconds:D2}";
    }

    private void OnRecoveryClicked()
    {
        onRecovery?.Invoke();
    }

    protected override void OnAfterShow()
    {
        base.OnAfterShow();
        Refresh();
    }
}
