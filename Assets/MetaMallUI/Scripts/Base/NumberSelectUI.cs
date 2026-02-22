using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 汎用の数値選択UI。インクリメント/デクリメント/Min/Max ボタン + 直接入力で値を変更する。
/// </summary>
public class NumberSelectUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private MyButton incrementButton;
    [SerializeField] private MyButton decrementButton;
    [SerializeField] private MyButton minButton;
    [SerializeField] private MyButton maxButton;

    [Header("色")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color lockedColor = Color.gray;

    private int value;
    private int minValue;
    private int maxValue;
    private Action<int> onValueChanged;

    public int Value => value;

    private void Awake()
    {
        if (incrementButton != null) incrementButton.SetOnClick(Increment);
        if (decrementButton != null) decrementButton.SetOnClick(Decrement);
        if (minButton != null) minButton.SetOnClick(ToMin);
        if (maxButton != null) maxButton.SetOnClick(ToMax);

        if (inputField != null)
        {
            inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            inputField.onEndEdit.AddListener(OnEndEdit);
        }
    }

    /// <summary>範囲と初期値を設定する。</summary>
    public void Setup(int min, int max, int initial = -1, Action<int> onValueChanged = null)
    {
        minValue = min;
        maxValue = Mathf.Max(min, max);
        this.onValueChanged = onValueChanged;
        SetValue(initial < 0 ? minValue : initial);
    }

    public void SetValue(int newValue)
    {
        value = Mathf.Clamp(newValue, minValue, maxValue);
        if (inputField != null) inputField.SetTextWithoutNotify(value.ToString());
        UpdateButtons();
        onValueChanged?.Invoke(value);
    }

    private void OnEndEdit(string text)
    {
        if (int.TryParse(text, out int parsed))
            SetValue(parsed);
        else
            SetValue(value);
    }

    private void Increment() => SetValue(value + 1);
    private void Decrement() => SetValue(value - 1);
    private void ToMin() => SetValue(minValue);
    private void ToMax() => SetValue(maxValue);

    private void UpdateButtons()
    {
        bool atMin = value <= minValue;
        bool atMax = value >= maxValue;
        SetButtonState(decrementButton, atMin);
        SetButtonState(minButton, atMin);
        SetButtonState(incrementButton, atMax);
        SetButtonState(maxButton, atMax);
    }

    private void SetButtonState(MyButton button, bool locked)
    {
        if (button == null) return;
        button.IsLocked = locked;
        var graphic = button.GetComponentInChildren<Graphic>();
        if (graphic != null) graphic.color = locked ? lockedColor : normalColor;
    }
}
