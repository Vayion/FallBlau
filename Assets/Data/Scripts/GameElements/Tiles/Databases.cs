using UnityEngine;

public class Databases : MonoBehaviour
{
    [SerializeField] private TerrainDatabase terrainDatabase;

    private static Databases instance;

    private void Awake()
    {
        instance = this;
    }

    public static TerrainSO GetTerrainSO(int id)
    {
        TerrainSO terrainSO = null;

        if(!instance.terrainDatabase.TerrainDictionary.TryGetValue(id, out terrainSO))
        {
            Debug.LogError("Terrain ID not found!");
        }

        return terrainSO;
    }

    public static TerrainSO GetTerrainSO(Terrain.TerrainType terrainType)
    {
        TerrainSO terrainSO = null;

        if (!instance.terrainDatabase.TerrainDictionary.TryGetValue((int)terrainType, out terrainSO))
        {
            Debug.LogError("Terrain ID not found!");
        }

        return terrainSO;
    }

}

