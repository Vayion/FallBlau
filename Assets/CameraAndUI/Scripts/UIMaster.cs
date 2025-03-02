using UnityEngine;

public class UIMaster : MonoBehaviour
{
    public static UIMaster instance;

    [SerializeField] private UITileInformation tileInfo;
    [SerializeField] private UICountryInfo countryInfo;
    [SerializeField] private UIDivisionManager divisionManager;
    private enum UIShown
    {
        none,
        tileInfo,
        countryInfo,
        addDivision
    }

    private UIShown currentActiveUI;
    public void Awake()
    {
        instance = this;
    }

    public static void CloseActive()
    {
        switch (instance.currentActiveUI)
        {
            case UIShown.tileInfo:
                instance.HideTileVisual();
                break;
            case UIShown.countryInfo:
                instance.HideCountryInfo(); 
                break;
            case UIShown.addDivision:
                instance.HideDivisionAddMenu();
                break;
        }

        instance.currentActiveUI = UIShown.none;
    }


    /// 
    /// tile info
    /// 

    public static void ShowTileVisual()
    {
        if(instance.currentActiveUI != UIShown.tileInfo)
        {
            CloseActive();
        }

        instance.tileInfo.parentObject.SetActive(true);
        instance.currentActiveUI = UIShown.tileInfo;
    }

    public static void UpdateTileVisual(TileVisual tile)
    {
        instance.tileInfo.UpdateTileVisual(tile);
    }

    private void HideTileVisual()
    {
        tileInfo.parentObject.SetActive(false);
    }

    public static bool TileInfoShownState()
    {
        return instance.currentActiveUI == UIShown.tileInfo;
    }

    ///
    /// country info
    ///

    public static void ShowCountryInfo()
    {
        if (instance.currentActiveUI != UIShown.countryInfo)
        {
            CloseActive();
        }

        instance.countryInfo.parentObject.SetActive(true);
        instance.countryInfo.UpdateCountryInfo(CountryLoader.countries[Player.GetPlayer().country]);
        instance.currentActiveUI = UIShown.countryInfo;
        PlayerInteraction.UnselectAll();
    }
    private void HideCountryInfo()
    {
        instance.countryInfo.Hide();
    }

    /// <summary>
    /// division add
    /// </summary>
    /// 

    public static void ShowDivisionAddMenu()
    {
        if (instance.currentActiveUI != UIShown.addDivision)
        {
            CloseActive();
        }

        instance.divisionManager.parentObject.SetActive(true);
        instance.divisionManager.UpdateInfo(CountryLoader.countries[Player.GetPlayer().country]);
        instance.currentActiveUI = UIShown.addDivision;
        PlayerInteraction.UnselectAll();
    }

    private void HideDivisionAddMenu()
    {
        instance.divisionManager.Hide();
    }

}
