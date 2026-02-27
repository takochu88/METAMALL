using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StabilityUI : MonoBehaviour
{
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Image fillImage;
    [SerializeField] private MyButton myButton;

    private int displayedAmount = -1;

    public void Setup(System.Action onClick)
    {
        if (myButton != null) myButton.SetOnClick(() => onClick?.Invoke());
        Refresh();
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
        var data = Mgr.Save.StabilityData;
        if (data.IsFull) return;

        int current = data.Amount;
        if (current != displayedAmount)
            UpdateAmount(data, current);

        UpdateTimer();
    }

    private void Refresh()
    {
        var data = Mgr.Save.StabilityData;
        UpdateAmount(data, data.Amount);
        UpdateTimer();
    }

    private void UpdateAmount(StabilityData data, int current)
    {
        displayedAmount = current;
        amountText.text = $"{current}/{data.Max}";
        if (fillImage != null)
            fillImage.fillAmount = data.Max > 0 ? (float)current / data.Max : 0f;
    }

    private void UpdateTimer()
    {
        var data = Mgr.Save.StabilityData;
        bool showTimer = !data.IsFull;
        timerText.gameObject.SetActiveIfChanged(showTimer);
        if (!showTimer) return;

        var remaining = data.GetTimeToNext();
        timerText.text = $"{(int)remaining.TotalMinutes:D2}:{remaining.Seconds:D2}";
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActiveIfChanged(visible);
    }
}
