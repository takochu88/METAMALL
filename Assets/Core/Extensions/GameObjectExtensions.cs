using UnityEngine;

public static class GameObjectExtensions
{
    public static void SetActiveIfChanged(this GameObject obj, bool isActive)
    {
        if (obj.activeSelf == isActive) return;
        obj.SetActive(isActive);
    }
}
