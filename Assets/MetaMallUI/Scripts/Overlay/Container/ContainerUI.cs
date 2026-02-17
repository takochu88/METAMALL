using TMPro;
using UnityEngine;

public class ContainerUI : OverlayUIBase
{
    [SerializeField] MyScrollRect scroll;
    [SerializeField] TMP_Text titleLabel;
    [SerializeField] MyButton openButton;

    private ContainerData containerData;

    public void SetData(ContainerData data)
    {
        containerData = data;
    }

    protected override void OnAfterShow()
    {
        titleLabel.text = containerData.Master.Name;
        Canvas.ForceUpdateCanvases();
        scroll.Init(containerData.rewards.Count, (index, cell) =>
        {
            //cell.Get<ContainerRowUI>().SetData(containerData.rewards[index]);
        });
    }
}
