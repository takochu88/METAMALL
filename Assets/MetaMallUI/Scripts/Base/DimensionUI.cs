using TMPro;
using UnityEngine;

/// <summary>
/// 次元（レアリティ）に応じた色を背景・枠・ラベルに反映する汎用コンポーネント。
/// </summary>
public class DimensionUI : MonoBehaviour
{
    [SerializeField] private MyButton button;
    [SerializeField] private TMP_Text label;

    public void SetDimension(Dimension type)
    {
        label.text = type.Name();
        label.color = type.Color();
    }

    private void OnClick()
    {
        // TODO 未実装
    }
}
