using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalentRowUI : MonoBehaviour
{
    [SerializeField] TMP_Text nameLabel;
    [SerializeField] TMP_Text baseValueLabel;
    [SerializeField] TMP_Text bonusValueLabel;
    [SerializeField] Image bonusBar;
    [SerializeField] Image baseBar;
    [SerializeField] MyButton upgradeButton;
    [SerializeField] GameObject plusIcon;

    private int index;
    private Action<int> onUpgrade;

    public void SetData(string name, int baseValue, int bonusValue, int maxValue, bool canUpgrade)
    {
        nameLabel.text = name;
        baseValueLabel.text = baseValue.ToString();

        if (bonusValue > 0)
        {
            bonusValueLabel.text = $"+{bonusValue}";
            bonusValueLabel.gameObject.SetActiveIfChanged(true);
        }
        else
        {
            bonusValueLabel.gameObject.SetActiveIfChanged(false);
        }

        float max = Mathf.Max(maxValue, 1);
        baseBar.fillAmount = baseValue / max;
        bonusBar.fillAmount = (baseValue + bonusValue) / max;

        upgradeButton.IsLocked = !canUpgrade;
        plusIcon.SetActiveIfChanged(canUpgrade);
    }

    public void SetOnUpgrade(int index, Action<int> onUpgrade)
    {
        this.index = index;
        this.onUpgrade = onUpgrade;
        upgradeButton.SetOnClick(OnClickUpgrade);
    }

    private void OnClickUpgrade()
    {
        onUpgrade?.Invoke(index);
    }
}
