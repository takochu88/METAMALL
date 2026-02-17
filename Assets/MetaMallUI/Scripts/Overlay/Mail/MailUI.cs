using UnityEngine;

public class MailUI : OverlayUIBase
{
    [SerializeField] private MyScrollRect scroll;

    protected override void OnAfterShow()
    {
        Canvas.ForceUpdateCanvases();
        scroll.Init(100, (index, cell) =>
        {
            cell.Get<MailRowUI>().SetData(index);
        });
    }
}
