using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    [SerializeField] string key;

    TMP_Text _text;
    TMP_Text text => _text ??= GetComponent<TMP_Text>();

    void Start()
    {
        Apply();
    }

    public void Apply()
    {
        if (string.IsNullOrEmpty(key)) return;
        text.text = Mgr.Local.Get(key);
    }

    public void SetKey(string newKey)
    {
        key = newKey;
        Apply();
    }
}
