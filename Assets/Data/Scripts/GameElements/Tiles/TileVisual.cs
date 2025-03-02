using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileVisual : MonoBehaviour
{
    public int tileId { get; private set; }

    public List<int> divisionVisualIds = new List<int>();

    private Terrain.TerrainType terrainId;

    [SerializeField] private Renderer tileBackgroundRenderer;
    [SerializeField] Material selected, selected_other, unselected;
    [SerializeField] GameObject foregroundTile, backgroundTile;

    public TileVisual Setup(Terrain.TerrainType id, int tileId_)
    {
        terrainId = id;
        tileId = tileId_;
        HideHighlight();

        return this;
    }

    public void OnTileSelect()
    {
        ShowHighlight();
    }

    public void OnTileDeselect()
    {
        HideHighlight();
    }

    private void ShowHighlight()
    {
        tileBackgroundRenderer.sharedMaterial = selected;
    }

    private void HideHighlight()
    {
        tileBackgroundRenderer.sharedMaterial = unselected;
    }

    public Terrain.TerrainType GetTerrainType()
    {
        return terrainId;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
