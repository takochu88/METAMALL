using UnityEngine;
using UnityEngine.Serialization;

public class SubMenuUI : MonoBehaviour
{
    [FormerlySerializedAs("selectInbentryMenuUI")] public SelectMenuRowUI selectInventoryMenuUI;
    public SelectMenuRowUI selectTipsMenuUI;
    public SelectMenuRowUI selectCollectionMenuUI;
    public SelectMenuRowUI selectMissionMenuUI;
    public SelectMenuRowUI selectShopMenuUI;
    public SelectMenuRowUI selectSeasonPassMenuUI;
    public SelectMenuRowUI selectLoginBonusMenuUI;
}
