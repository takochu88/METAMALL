using UnityEngine;

[DefaultExecutionOrder(-100)]
public class Mgr : MonoBehaviour
{
    public static Mgr Instance { get; private set; }

    private bool isSetup;

    [SerializeField] private MasterManager master;
    [SerializeField] private SaveManager save;
    [SerializeField] private LocalManager local;
    [SerializeField] private ResourceManager resource;
    [SerializeField] private ToastManager toast;
    [SerializeField] private LoadingManager loading;
    [SerializeField] private PlayFabManager playFab;

    public static MasterManager Master => Instance.master;
    public static SaveManager Save => Instance.save;
    public static LocalManager Local => Instance.local;
    public static ResourceManager Resource => Instance.resource;
    public static ToastManager Toast => Instance.toast;
    public static LoadingManager Loading => Instance.loading;
    public static PlayFabManager PlayFab => Instance.playFab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Setup(bool isForce = false)
    {
        if(isSetup && !isForce) return;

        isSetup = true;
        master.Init();
        save.Load();
        local.Init();
    }
}
