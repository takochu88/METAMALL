using Cysharp.Threading.Tasks;
using UnityEngine;
using PrimeTween;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public UseCases useCase;
    private Image target;
    void  Awake()
    {
       
    }

    public async UniTask a()
    {
        Debug.Log("[Main] Awake");
        await Tween.Alpha(target, 0, 1);
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
