using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugRewardRowUI : MonoBehaviour
{
    [SerializeField] TMP_InputField rewardIdInput;
    [SerializeField] MyButton rewardIdUpButton;
    [SerializeField] MyButton rewardIdDownButton;
    [SerializeField] TMP_InputField amountInput;
    [SerializeField] MyButton amountUpButton;
    [SerializeField] MyButton amountDownButton;
    [SerializeField] MyButton executeButton;
    [SerializeField] MyButton hintButton;
    [SerializeField] TMP_Text hintLabel;
    [SerializeField] TMP_Text previewLabel;
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text nameLabel;
    [SerializeField] RewardIconUI rewardIconUI;

    Main main;
    List<int> validRewardIds;

    public void Setup(Main main)
    {
        this.main = main;
        executeButton.SetOnClick(OnExecute);
        rewardIdInput.onValueChanged.AddListener(OnRewardIdChanged);

        rewardIdUpButton.SetOnClick(() => StepRewardId(1));
        rewardIdDownButton.SetOnClick(() => StepRewardId(-1));
        amountUpButton.SetOnClick(() => StepAmount(1));
        amountDownButton.SetOnClick(() => StepAmount(-1));

        hintLabel.text = BuildHintText();
        hintLabel.gameObject.SetActive(false);
        hintButton.SetOnClick(() => hintLabel.gameObject.SetActive(!hintLabel.gameObject.activeSelf));
    }

    public void SetDefault()
    {
        if (validRewardIds == null) BuildValidRewardIds();
        if (validRewardIds.Count > 0)
            rewardIdInput.text = validRewardIds[0].ToString();
        else
            OnRewardIdChanged("");
        amountInput.text = "1";
    }

    void OnRewardIdChanged(string value)
    {
        if (!int.TryParse(value, out var rewardId))
        {
            previewLabel.text = "Empty";
            iconImage.gameObject.SetActive(false);
            nameLabel.gameObject.SetActive(false);
            return;
        }

        var name = ResolveName(rewardId);
        bool found = !string.IsNullOrEmpty(name);
        previewLabel.text = found ? name : "Empty";
        iconImage.gameObject.SetActive(found);
        nameLabel.text = found ? name : "";
        nameLabel.gameObject.SetActive(found);
       // rewardIconUI.SetData(rewardId, -1,  showAmount: false);
    }

    static string ResolveName(int rewardId)
    {
        if (Mgr.Master.TryGetReward(rewardId, out var reward))
            return reward.Name;
        return null;
    }

    // ── 有効なRewardIDリストを構築 ──
    void BuildValidRewardIds()
    {
        validRewardIds = new List<int>(Mgr.Master.AllRewardMasters.Keys);
        validRewardIds.Sort();
    }

    // ── RewardID: 有効なIDリスト内を循環 ──
    void StepRewardId(int direction)
    {
        if (validRewardIds == null) BuildValidRewardIds();
        if (validRewardIds.Count == 0) return;

        int current = 0;
        if (!string.IsNullOrEmpty(rewardIdInput.text))
            int.TryParse(rewardIdInput.text, out current);

        int index = validRewardIds.BinarySearch(current);
        if (index < 0)
        {
            index = ~index;
            if (direction < 0) index--;
            index = (index % validRewardIds.Count + validRewardIds.Count) % validRewardIds.Count;
        }
        else
        {
            index = (index + direction % validRewardIds.Count + validRewardIds.Count) % validRewardIds.Count;
        }

        rewardIdInput.text = validRewardIds[index].ToString();
      //  rewardIconUI.SetData(validRewardIds[index]);
    }

    // ── Amount: 1ずつ増減 ──
    void StepAmount(int delta)
    {
        int current = 1;
        if (!string.IsNullOrEmpty(amountInput.text))
            int.TryParse(amountInput.text, out current);
        current = Mathf.Max(1, current + delta);
        amountInput.text = current.ToString();
    }

    void OnExecute()
    {
        if (!int.TryParse(rewardIdInput.text, out var rewardId))
        {
            Mgr.Toast.Show("RewardIdを数値で入力してください");
            return;
        }

        int amount = 1;
        if (!string.IsNullOrEmpty(amountInput.text) && !int.TryParse(amountInput.text, out amount))
        {
            Mgr.Toast.Show("個数を数値で入力してください");
            return;
        }
        
        main.useCase.reward.AddReward(rewardId, amount, isActive: true);
    }

    static string BuildHintText()
    {
        var sb = new StringBuilder();
        foreach (RewardType type in Enum.GetValues(typeof(RewardType)))
        {
            sb.AppendLine($"{(int)type * RewardBase.Multiplier}~ {type}");
        }
        return sb.ToString().TrimEnd();
    }
}
