using UnityEngine;

public class MetaMallUI : MonoBehaviour
{
    [SerializeField] private MenuUIBase[] menuUIs;
    [SerializeField] private OverlayStack overlayStack;
    [SerializeField] private CanvasGroup canvasGroup;

    public SelectMainMenuUI selectMainMenuUI;

    public OverlayStack OverlayStack => overlayStack;
    public TopMenuUI TopMenuUI => menuUIs[0] as TopMenuUI;
    public StageMenuUI StageMenuUI => menuUIs[1] as StageMenuUI;
    public GameEventMenuUI GameEventMenuUI => menuUIs[2] as GameEventMenuUI;
    public PurchaseMenuUI PurchaseMenuUI => menuUIs[3] as PurchaseMenuUI;
    public GachaMenuUI GachaMenuUI => menuUIs[4] as GachaMenuUI;
    public UpperMenuUI upperMenuUI;
    public FooterMenuUI footerMenuUI;

    public void Setup(Main main)
    {

    }

    public MenuUIBase GetMenuUI(MainMenuType mainMenuType)
    {
        return menuUIs[(int)mainMenuType];
    }

    public bool IsInteractable => canvasGroup.interactable;

    public void SetInteractable(bool interactable)
    {
        canvasGroup.interactable = interactable;
    }
}
