using System;
using TMPro;
using UnityEngine;

public class DebugFlagRowUI : MonoBehaviour
{
    [SerializeField] TMP_InputField flagInput;
    [SerializeField] MyButton setOnButton;
    [SerializeField] MyButton setOffButton;

    public void Setup()
    {
        setOnButton.SetOnClick(() => Execute(true));
        setOffButton.SetOnClick(() => Execute(false));
    }

    void Execute(bool on)
    {
        if (!TryParseFlag(flagInput.text, out var flag)) return;

        if (on)
            Mgr.Save.FlagData.SetOn(flag);
        else
            Mgr.Save.FlagData.SetOff(flag);

        Mgr.Toast.Show($"{flag} = {(on ? "ON" : "OFF")}");
    }

    bool TryParseFlag(string input, out FlagType flag)
    {
        if (Enum.TryParse(input, true, out flag)) return true;
        if (int.TryParse(input, out var intVal)) { flag = (FlagType)intVal; return true; }
        Mgr.Toast.Show("FlagType名 または 数値を入力してください");
        flag = default;
        return false;
    }
}
