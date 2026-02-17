using UnityEngine;

public class UseCase_Container : MonoBehaviour
{
    private Main main;

    public void Setup(Main main)
    {
        this.main = main;
    }

    /// <summary> コンテナを開封してリワードを適用・UI表示 </summary>
    public void OpenContainer(ContainerData container)
    {
        if (container.isOpened) return;
        container.isOpened = true;
    }
}
