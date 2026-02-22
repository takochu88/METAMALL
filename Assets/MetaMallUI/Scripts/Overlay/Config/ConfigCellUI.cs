using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfigCellUI : CellUIBase
{
    [SerializeField] private TMP_Text valueText;

    [Header("Slider")]
    [SerializeField] private GameObject sliderRoot;
    [SerializeField] private Slider slider;

    [Header("Check")]
    [SerializeField] private GameObject checkRoot;
    [SerializeField] private MyButton checkOnButton;
    [SerializeField] private MyButton checkOffButton;

    [Header("Toggle")]
    [SerializeField] private GameObject toggleRoot;
    [SerializeField] private MyButton[] toggleButtons;
    [SerializeField] private TMP_Text[] toggleLabels;

    [Header("Button")]
    [SerializeField] private GameObject buttonRoot;
    [SerializeField] private MyButton actionButton;
    [SerializeField] private TMP_Text actionButtonLabel;

    [Header("Label")]
    [SerializeField] private GameObject labelRoot;
    [SerializeField] private TMP_Text labelValueText;

    private ConfigMaster master;
    private float currentValue;
    private Action<string, float> onValueChanged;
    private Action<string> onButtonClicked;

    public void Setup(Action<string, float> onValueChanged, Action<string> onButtonClicked)
    {
        this.onValueChanged = onValueChanged;
        this.onButtonClicked = onButtonClicked;
    }

    public void SetData(ConfigMaster master, float value)
    {
        this.master = master;
        this.currentValue = value;

        Title.text = master.Name;

        HideAllOptions();

        switch (master.configOptionType)
        {
            case ConfigOptionType.Slider:
                SetupSlider(value);
                break;
            case ConfigOptionType.Check:
                SetupCheck(value);
                break;
            case ConfigOptionType.Toggle2:
                SetupToggle(value, 2);
                break;
            case ConfigOptionType.Toggle3:
                SetupToggle(value, 3);
                break;
            case ConfigOptionType.Toggle4:
                SetupToggle(value, 4);
                break;
            case ConfigOptionType.Button:
                SetupButton();
                break;
            case ConfigOptionType.Label:
                SetupLabel();
                break;
        }
    }

    private void HideAllOptions()
    {
        if (sliderRoot != null) sliderRoot.SetActiveIfChanged(false);
        if (checkRoot != null) checkRoot.SetActiveIfChanged(false);
        if (toggleRoot != null) toggleRoot.SetActiveIfChanged(false);
        if (buttonRoot != null) buttonRoot.SetActiveIfChanged(false);
        if (labelRoot != null) labelRoot.SetActiveIfChanged(false);
    }

    // ── Slider ──────────────────────────────
    private void SetupSlider(float value)
    {
        if (sliderRoot == null || slider == null) return;
        sliderRoot.SetActiveIfChanged(true);

        slider.onValueChanged.RemoveAllListeners();
        slider.minValue = master.min;
        slider.maxValue = master.max;
        slider.value = value;
        slider.onValueChanged.AddListener(OnSliderChanged);

        UpdateValueText(value);
    }

    private void OnSliderChanged(float value)
    {
        currentValue = value;
        UpdateValueText(value);
        onValueChanged?.Invoke(master.key, value);
    }

    private void UpdateValueText(float value)
    {
        if (valueText != null)
            valueText.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    // ── Check ───────────────────────────────
    private void SetupCheck(float value)
    {
        if (checkRoot == null) return;
        checkRoot.SetActiveIfChanged(true);

        bool isOn = value >= 1f;
        RefreshCheckButtons(isOn);

        if (checkOnButton != null)
            checkOnButton.SetOnClick(() => OnCheckChanged(true));
        if (checkOffButton != null)
            checkOffButton.SetOnClick(() => OnCheckChanged(false));
    }

    private void OnCheckChanged(bool isOn)
    {
        currentValue = isOn ? 1f : 0f;
        RefreshCheckButtons(isOn);
        onValueChanged?.Invoke(master.key, currentValue);
    }

    private void RefreshCheckButtons(bool isOn)
    {
        if (checkOnButton != null)
            checkOnButton.GetComponent<Image>().color = isOn ? CellUIBase.SelectedColor : CellUIBase.NormalColor;
        if (checkOffButton != null)
            checkOffButton.GetComponent<Image>().color = !isOn ? CellUIBase.SelectedColor : CellUIBase.NormalColor;
    }

    // ── Toggle ──────────────────────────────
    private void SetupToggle(float value, int count)
    {
        if (toggleRoot == null || toggleButtons == null) return;
        toggleRoot.SetActiveIfChanged(true);

        int selected = Mathf.RoundToInt(value);

        for (int i = 0; i < toggleButtons.Length; i++)
        {
            if (toggleButtons[i] == null) continue;

            bool visible = i < count;
            toggleButtons[i].gameObject.SetActiveIfChanged(visible);

            if (!visible) continue;

            if (toggleLabels != null && i < toggleLabels.Length && toggleLabels[i] != null)
            {
                string label = (master.choiceKeys != null && i < master.choiceKeys.Length)
                    ? Mgr.Local.Get(master.choiceKeys[i])
                    : i.ToString();
                toggleLabels[i].text = label;
            }

            toggleButtons[i].GetComponent<Image>().color =
                (i == selected) ? CellUIBase.SelectedColor : CellUIBase.NormalColor;

            int idx = i;
            toggleButtons[i].SetOnClick(() => OnToggleChanged(idx));
        }
    }

    private void OnToggleChanged(int index)
    {
        currentValue = index;
        int count = master.configOptionType switch
        {
            ConfigOptionType.Toggle2 => 2,
            ConfigOptionType.Toggle3 => 3,
            ConfigOptionType.Toggle4 => 4,
            _ => 2,
        };
        RefreshToggleColors(index, count);
        onValueChanged?.Invoke(master.key, currentValue);
    }

    private void RefreshToggleColors(int selected, int count)
    {
        if (toggleButtons == null) return;
        for (int i = 0; i < toggleButtons.Length && i < count; i++)
        {
            if (toggleButtons[i] == null) continue;
            toggleButtons[i].GetComponent<Image>().color =
                (i == selected) ? CellUIBase.SelectedColor : CellUIBase.NormalColor;
        }
    }

    // ── Button ──────────────────────────────
    private void SetupButton()
    {
        if (buttonRoot == null) return;
        buttonRoot.SetActiveIfChanged(true);

        if (actionButtonLabel != null)
            actionButtonLabel.text = master.Desc;

        if (actionButton != null)
            actionButton.SetOnClick(() => onButtonClicked?.Invoke(master.key));
    }

    // ── Label ───────────────────────────────
    private void SetupLabel()
    {
        if (labelRoot == null) return;
        labelRoot.SetActiveIfChanged(true);

        if (labelValueText != null)
            labelValueText.text = master.Desc;
    }
}
