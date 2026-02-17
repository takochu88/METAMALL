using System;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class Main : MonoBehaviour
{
    public UseCases useCase;
    public MetaMallUI ui;
  
    void  Awake()
    {
        Mgr.Instance.Setup();
        useCase.Setup(this);
        ui.Setup(this);
    }

    private void Update()
    {
        useCase.popup.OnUpdate();
        useCase.debug.OnUpdate();
    }
}
