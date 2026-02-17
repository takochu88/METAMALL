using TMPro;
using UnityEngine;

public class MailRowUI : MonoBehaviour
{
    [SerializeField] TMP_Text label;

    public void SetData(int index)
    {
        label.text = $"メール {index + 1}";
    }
}
