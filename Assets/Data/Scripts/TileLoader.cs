using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TileLoader : MonoBehaviour
{

    public TileDesignManager tileDesignManager;

    public Texture2D terrainMap;
    [SerializeField] private Texture2D countryMap;
    private int imageScale = Defines.tileSize;
    [SerializeField] private ForestObjects forestObjects;
    [SerializeField] private MapHandler mapHandler;
    [SerializeField] private CountryLoader countryLoader;

    
    public List<TileVisual> tileVisuals;
    public List<Tile> tiles;

    //  
    //  ____________1       ^
    //  \    /\    /        |
    //   \  /  \  /         |   height² + (radius/2)² = radius²;
    //    \/____\/          | ->height = Sqrt(0.75)
    //    2                 |

    //radius = 1

    private double height_, scale_, xDistance, yDistance;

    // called as soon as the game loads

    private void Start()
    {
        countryLoader.LoadCountries();

        tileDesignManager = FindObjectOfType<TileDesignManager>();

        if (tileDesignManager == null) Debug.LogError("TileDesignManager not found in the scene!");
        height_ = Math.Sqrt(0.75d);
        scale_ = 1d / height_;
        yDistance = scale_ * 1.5d;
        xDistance = 2d;
        
        
        tiles = new List<Tile>();
        tileVisuals = new List<TileVisual>();

        int i = 0, j = 0, id_ = 0;
        for (double x = 0; x < terrainMap.height; i++, x = i*imageScale*xDistance)
        {
            j = 0;
            for (double y = 0; y < terrainMap.width; j++, y = j*imageScale*yDistance, id_++)
            {
                Terrain.TerrainType terrainType = TileDesignManager.GetTerrainTypeFromHex(((int)(terrainMap.GetPixel((int)x, (int)y).r * 255) << 16) | 
                                                                ((int)(terrainMap.GetPixel((int)x, (int)y).g * 255) << 8) | 
                                                                (int)(terrainMap.GetPixel((int)x, (int)y).b * 255));
                var newLocation = new Vector3((j % 2 == 0 ? 1f : 0f) + (float)(xDistance * i), 0f, (float)(yDistance * j));

                // create tile visual
                GameObject tileVisual = tileDesignManager.getRandomTexture(terrainType, newLocation, transform);
                tileVisual.transform.localScale = new Vector3((float)scale_, (float)scale_, (float)scale_);
                tileVisual.transform.GetChild(1).GetComponent<TileVisual>().Setup(terrainType, id_);
                tileVisuals.Add(tileVisual.transform.GetChild(1).GetComponent<TileVisual>());

                // create tile
                Tile tile = new Tile();

                // get country
                Country country = countryLoader.GetCountryByColor(countryMap.GetPixel((int)x, (int)y));

                tile.Setup(id_, terrainType, tileVisual.transform.position, country);
                tiles.Add(tile);
                
            }
        }

        VisualManager.CollectTileData(tileVisuals);
        mapHandler.generateMap(tiles,i,j);
        TheGameManager.CollectTileData(tiles);
        forestObjects.StartGeneration(this);

    }

    public Texture2D GetMapSettings()
    {
        return terrainMap;
    }

}