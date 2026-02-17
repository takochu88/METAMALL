using UnityEngine;

public class UseCase_Mall : MonoBehaviour
{
    private Main main;

    public void Setup(Main main)
    {
        this.main = main;
        main.ui.upperMenuUI.selectMailUI.SetupSub(OnShowMail);
    }

    public void OnShowMail()
    {
        main.ui.OverlayStack.mailUI.Show();
    }

    public void OnHideMail()
    {
        
    }
}
