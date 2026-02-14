using UnityEngine;

public class Mgr : MonoBehaviour
{
    public static Mgr Instance { get; private set; }

    [SerializeField] private MasterManager master;
    [SerializeField] private SaveManager save;
    [SerializeField] private LocalManager local;

    public static MasterManager Master => Instance.master;
    public static SaveManager Save => Instance.save;
    public static LocalManager Local => Instance.local;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        save.Load();
        local.Init();
        Debug.Log("[Mgr] Awake");
    }
}
