using UnityEngine;

public class TestScroll : MonoBehaviour
{
    [SerializeField] private MyScrollRect scroll;
    [SerializeField] private int itemCount = 100;

    void Start()
    {
        scroll.Init(itemCount, (index, cell) =>
        {
            cell.Get<TestCell>().Set(index);
        });
    }
}
