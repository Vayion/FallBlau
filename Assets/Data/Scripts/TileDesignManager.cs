using System.Collections.Generic;
using UnityEngine;

public class TileDesignManager : MonoBehaviour
{
    

    public GameObject tileObject;


    private const int desertHex = 0xc8853c;
    private const int plainsHex = 0x94bb2b;
    private const int mountainsHex = 0x838889;
    private const int forestHex = 0x26651f;
    private const int waterHex = 0x547c9f;

    public GameObject getRandomTexture(Terrain.TerrainType type, Vector3 position, Transform parent)
    {
        //var randomRotation = Random.Range(0, 6);
        //var rotation = new Vector3(0f, randomRotation * 60f, 0f);
        var tileInstance = Instantiate(tileObject, position, Quaternion.identity, parent);

        //tileInstance.transform.Rotate(rotation);

        // Find the foreground-hexagon child
        var foregroundHexagon = tileInstance.transform.Find("foreground-hexagon");
        if (foregroundHexagon == null)
        {
            Debug.LogError("The prefab does not have a child named 'foreground-hexagon'!");
            return null;
        }

        // Get the Renderer component of the foreground-hexagon
        var tileRenderer = foregroundHexagon.GetComponent<Renderer>();
        if (tileRenderer == null)
        {
            Debug.LogError("The 'foreground-hexagon' does not have a Renderer component!");
            return null;
        }

        // Select a random texture based on the terrain type
        Texture selectedTexture = null;

        //get texture from database
        TerrainSO terrainSO = Databases.GetTerrainSO(type);

        tileRenderer.material = terrainSO.terrainMaterial;

        //selectedTexture = terrainSO.terrainTextures[Random.Range(0, terrainSO.terrainTextures.Count)];

        //// Apply the selected texture to the material
        //if (selectedTexture != null)
        //    tileRenderer.material.mainTexture = selectedTexture;
        //else
        //    Debug.LogError("No texture found for the selected terrain type!");

        return tileInstance;
    }
    
    
    public static Terrain.TerrainType GetTerrainTypeFromHex(int hexValue)
    {
        Dictionary<int, Terrain.TerrainType> terrainColors = new Dictionary<int, Terrain.TerrainType>
        {
            { desertHex, Terrain.TerrainType.desert },
            { plainsHex, Terrain.TerrainType.plains },
            { mountainsHex, Terrain.TerrainType.mountains },
            { forestHex, Terrain.TerrainType.forest },
            { waterHex, Terrain.TerrainType.water }
        };

        int closestHex = desertHex; // Default to desert
        float closestDistance = float.MaxValue;

        foreach (var entry in terrainColors)
        {
            int r1 = (hexValue >> 16) & 0xFF;
            int g1 = (hexValue >> 8) & 0xFF;
            int b1 = hexValue & 0xFF;

            int r2 = (entry.Key >> 16) & 0xFF;
            int g2 = (entry.Key >> 8) & 0xFF;
            int b2 = entry.Key & 0xFF;

            float distance = Mathf.Sqrt(Mathf.Pow(r1 - r2, 2) + Mathf.Pow(g1 - g2, 2) + Mathf.Pow(b1 - b2, 2));

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestHex = entry.Key;
            }
        }

        return terrainColors[closestHex];
    }
}