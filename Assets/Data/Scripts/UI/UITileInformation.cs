using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITileInformation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI terrainType;
    [SerializeField] private Image terrainImageType;
    [SerializeField] private Image flag;
    public GameObject parentObject;

    private void Start()
    {
        parentObject.SetActive(false);
    }

    public void UpdateTileVisual(TileVisual tile)
    {
        TerrainSO terrain = Databases.GetTerrainSO(tile.GetTerrainType());

        if(terrain != null)
        {
            terrainType.SetText(terrain.terrainName);
            terrainImageType.sprite = terrain.tileInformationImage;
        }

        TileData tileData = VisualManager.GetTileData(tile.tileId);

        if(tileData.country < 0)
        {
            flag.gameObject.SetActive(false);
        }
        else
        {
            flag.gameObject.SetActive(true);
            Texture2D flagTexture = CountryLoader.countries[tileData.country].GetFlag();

            Sprite sprite = Sprite.Create(flagTexture, new Rect(0, 0, flagTexture.width, flagTexture.height), new Vector2(0.5f, 0.5f));
            flag.sprite = sprite;
        }
    }

    public bool ShownState()
    {
        return parentObject.activeSelf;
    }
}
