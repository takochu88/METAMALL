using TMPro;
using UnityEngine;

public class TestCell : MonoBehaviour
{
    [SerializeField] private TMP_Text indexText;

    public void Set(int index)
    {
        indexText.text = $"#{index}";
    }
}
