using System.Collections.Generic;
using UnityEngine;

public class VisualManager : MonoBehaviour
{
    public static VisualManager instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject divisionVisual;
    [SerializeField] private HexagonMeshGenerator countryOverlay;

    private Dictionary<int, DivisionData> localDivisions = new Dictionary<int, DivisionData>();
    private Dictionary<int, DivisionVisual> divisionVisuals = new Dictionary<int, DivisionVisual>();
    private Dictionary<int, int> divisionVisualPairs = new Dictionary<int, int>(); // division id to visual id
    private List<TileVisual> tileVisuals = new List<TileVisual>();

    public List<DivisionData> divisionDataListDebug = new List<DivisionData>();
    [SerializeField] private List<TileData> localTiles = new List<TileData>();

    public static void UpdateDivision(DivisionData data)
    {
        // get old division data if it exists
        DivisionData oldDivisionData = null;

        if(instance.localDivisions.TryGetValue(data.id, out oldDivisionData)) 
        {
            instance.localDivisions[data.id] = data;
        }
        else
        {
            instance.localDivisions.Add(data.id, data);
        }

        // if old division doesnt exist
        if(oldDivisionData == null)
        {
            SetVisualToTile(data, oldDivisionData);
        }
        // if division is in a difference place or has a different target
        else if(data.currentTileId != oldDivisionData.currentTileId || data.targetTileId != oldDivisionData.targetTileId)
        {
            SetVisualToTile(data, oldDivisionData);
        }


        //debug
        for (int i = 0; i < instance.divisionDataListDebug.Count; i++)
        {
            if (instance.divisionDataListDebug[i].id == data.id)
            {
                instance.divisionDataListDebug[i] = data;
                return;
            }
        }

        instance.divisionDataListDebug.Add(data);

    }

    private static void SetVisualToTile(DivisionData data, DivisionData oldDivisionData)
    {
        // if division exists
        if(oldDivisionData != null)
        {
            // if division has a visual
            if (instance.divisionVisualPairs.TryGetValue(data.id, out int oldVisualId))
            {
                // leave visual
                DivisionLeaveVisual(data.id, oldVisualId);
            }
        }

        // look for any visuals on this tile

        TileVisual tile = instance.tileVisuals[data.currentTileId];
        for (int i = 0; i < tile.divisionVisualIds.Count; i++)
        {
            // add to visual if there is another visual on the tile with the same target

            DivisionVisual visual = instance.divisionVisuals[tile.divisionVisualIds[i]];
            if(visual.targetId == data.targetTileId)
            {
                AddDivisionVisual(data, tile.divisionVisualIds[i]);
                return;
            }
        }

        // if there wasnt any visual on the tile
        SpawnDivisionVisual(data);
    }

    private static void SpawnDivisionVisual(DivisionData data)
    {
        GameObject newDivisionVisual = Instantiate(instance.divisionVisual);
        DivisionVisual visual = newDivisionVisual.GetComponent<DivisionVisual>();

        Vector3 tilePosition = CalculateVisualPosition(data.currentTileId, data.targetTileId, out Quaternion rotation);

        int newVisualId = FindUnusedKey();
        visual.Setup(new List<int>() { data.id }, tilePosition, rotation, newVisualId, data.currentTileId, data.targetTileId);
        instance.divisionVisuals.Add(newVisualId, visual);
        instance.divisionVisualPairs.Add(data.id, newVisualId);

        TileVisual tile = instance.tileVisuals[data.currentTileId];
        tile.divisionVisualIds.Add(newVisualId);
    }

    public static void DestroyVisual(int visualId)
    {
        GameObject visualObject = GetDivisionVisual(visualId).gameObject;

        int tileId = instance.divisionVisuals[visualId].GetTile();
        instance.divisionVisuals.Remove(visualId);
        instance.tileVisuals[tileId].divisionVisualIds.Remove(visualId);

        Destroy(visualObject);
    }

    private static void AddDivisionVisual(DivisionData data, int visualId)
    {
        DivisionVisual visual = instance.divisionVisuals[visualId];
        instance.divisionVisualPairs.Add(data.id, visualId);
        visual.AddDivision(data.id);
    }

    private static void DivisionLeaveVisual(int divisionId, int visualId)
    {
        DivisionVisual visual = instance.divisionVisuals[visualId];
        instance.divisionVisualPairs.Remove(divisionId);
        visual.RemoveDivision(divisionId);
    }

    public static void CollectTileData(List<TileVisual> visuals)
    {
        for (int i = 0; i < visuals.Count; i++)
        {
            instance.tileVisuals.Add(visuals[i]);
        }
    }

    public static void ClearVisuals()
    {
        PlayerInteraction.UnselectAll();

        int[] ids = new int[instance.divisionVisuals.Keys.Count];
        instance.divisionVisuals.Keys.CopyTo(ids, 0);

        for (int i = 0; i < ids.Length; i++)
        {
            {
                DestroyVisual(ids[i]);
            }
        }

        instance.divisionVisuals.Clear();
        instance.localDivisions.Clear();
        instance.divisionVisualPairs.Clear();
        instance.divisionDataListDebug.Clear();
    }

    public static Vector3 CalculateVisualPosition(int currentTileId, int targetTileId, out Quaternion rotation)
    {
        Vector3 tilePosition = GetTileVisualPosition(currentTileId);
        Vector3 targetPosition = GetTileVisualPosition(targetTileId);

        Vector3 offset = Vector3.Normalize(targetPosition - tilePosition) * 0.65f;
        rotation = Quaternion.identity;

        if (offset.magnitude < 0.1f)
        {
            rotation = Quaternion.LookRotation(new Vector3(0, 0, -1));
        }
        else
        {
            rotation = Quaternion.LookRotation(offset);
        }

        return tilePosition + offset;
    }

    public static DivisionVisual GetDivisionVisual(int divisionVisualId)
    {
        return instance.divisionVisuals[divisionVisualId];
    }

    public static DivisionVisual GetDivisionVisualFromDivisionId(int divisionId)
    {
        return instance.divisionVisuals[instance.divisionVisualPairs[divisionId]];
    }

    public static DivisionData GetDivisionData(int divisionId)
    {
        return instance.localDivisions[divisionId];
    }

    public static Vector3 GetTileVisualPosition(int tileId)
    {
        return instance.tileVisuals[tileId].transform.position;
    }

    ///
    /// Tiles
    ///

    public static void UpdateTiles(List<TileData> tiles)
    {
        instance.localTiles.AddRange(tiles);
        instance.countryOverlay.UpdateGrid(tiles);
    } 

    public static void UpdateTile(TileData tile)
    {
        instance.localTiles[tile.id] = tile;
        instance.countryOverlay.UpdateTile(tile.id, tile.country);

        for (int i = 0; i < tile.neighbors.Length; i++)
        {
            TileData neighbor = GetTileData(tile.neighbors[i]);
            instance.countryOverlay.UpdateTile(neighbor.id, neighbor.country);
        }
    }

    public static TileData GetTileData(int tileId)
    {
        return instance.localTiles[tileId];
    }

    /// 
    /// battles
    /// 

    public static void UpdateBattle(LandBattleData data)
    {
        UIBattleIconOverlay.instance.UpdateBattleIcon(data.battleProgress, data.tileId);
    }

    public static void RemoveBattle(LandBattleData data)
    {
        UIBattleIconOverlay.instance.RemoveBattleIcon(data.battleProgress, data.tileId);
    }

    static int nextKey = 0;
    static int FindUnusedKey()
    {
        nextKey++;
        return nextKey;
    }
}
