using UnityEngine;

public class UseCase_Stability : MonoBehaviour
{
    private Main main;
    private StabilityDetailUI detailUI;

    public void Setup(Main main)
    {
        this.main = main;
        detailUI = main.ui.OverlayStack.stabilityDetailUI;
        detailUI.Setup(OnRecovery);
        main.ui.footerMenuUI.stabilityUI.Setup(OnShowDetail);
    }

    public void OnShowDetail()
    {
        detailUI.Show();
    }

    private void OnRecovery()
    {
        Mgr.PlayFab.AddStability(10,
            onSuccess: () => Mgr.Toast.Show(Mgr.Local.Get("ui-stability-recovered")));
    }
}
