using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class PlayerInteraction : MonoBehaviour
{
    private bool isMouseOverUI;
    
    private TileVisual hoveredTile;
    private TileVisual selectedTile;
    
    private List<int> hoveredDivisions = new List<int>();
    private List<int> selectedDivisions = new List<int>();

    public static PlayerInteraction instance;
    public static bool cantInteract = false;

    public static event EventHandler<SelectedDivisionArgs> OnSelectedDivisionChanged;
    public static event EventHandler<SelectedTileArgs> OnTileSelect;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHoveredTile();
    }

    void UpdateHoveredTile()
    {
        ResetHover();
        isMouseOverUI = EventSystem.current.IsPointerOverGameObject();

        if (isMouseOverUI)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            if(hit.collider.TryGetComponent(out TileVisual tileVisual))
            {
                hoveredTile = tileVisual;
            }

            if (hit.collider.TryGetComponent(out DivisionVisual divisionVisual))
            {
                hoveredDivisions = divisionVisual.GetDivisionIds();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!cantInteract)
            {
                UnselectTiles();
                UnselectAllDivisions();

                SelectTile();
                SelectDivisions();
            }
            else
            {
                OnTileSelect?.Invoke(this, new SelectedTileArgs { selectedTileId = hoveredTile.tileId });
            }


        }

        if (Input.GetMouseButtonUp(1))
        {
            if (!cantInteract)
            {
                if (selectedDivisions.Count > 0 && hoveredTile != null)
                {
                    int targetId = hoveredTile.tileId;

                    TheGameManager.instance.RequestMoveDivisions(selectedDivisions, targetId);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!UIMaster.TileInfoShownState())
            {
                UnselectTiles();
                UnselectAllDivisions();
            }

            UIMaster.CloseActive();
        }

    }

    public void SelectDivisions()
    {
        if (hoveredDivisions.Count > 0)
        {
            UnselectAllDivisions(false);

            selectedDivisions.AddRange(hoveredDivisions);

            UnselectTiles();
            UIMaster.CloseActive();

            OnSelectedDivisionChanged?.Invoke(this, new SelectedDivisionArgs { selectedDivisionIds = selectedDivisions });
        }
    }

    public void SelectDivisions(List<int> divisionIds)
    {
        UnselectAllDivisions(false);

        selectedDivisions.AddRange(divisionIds);

        UnselectTiles();
        UIMaster.CloseActive();

        OnSelectedDivisionChanged?.Invoke(this, new SelectedDivisionArgs { selectedDivisionIds = selectedDivisions });
    }

    void UnselectAllDivisions(bool redraw = true)
    {
        if(selectedDivisions.Count > 0)
        {
            selectedDivisions.Clear();
            if (redraw) { OnSelectedDivisionChanged?.Invoke(this, new SelectedDivisionArgs { selectedDivisionIds = selectedDivisions }); }

        }

    }

    void SelectTile()
    {
        if(hoveredTile != null)
        {
            selectedTile = hoveredTile;
            selectedTile.OnTileSelect();

            UIMaster.ShowTileVisual();
            UIMaster.UpdateTileVisual(selectedTile);

            UnselectAllDivisions(false);
            OnTileSelect?.Invoke(this, new SelectedTileArgs
            {
                selectedTileId = selectedTile.tileId,
            });
        }


    }

    void UnselectTiles()
    {
        if(selectedTile != null)
        {
            selectedTile.OnTileDeselect();
            selectedTile = null;
        }

    }

    public static void UnselectAll()
    {
        instance.UnselectAllDivisions();
        instance.UnselectTiles();
    }

    public List<int> GetDivisionsSelected()
    {
        return selectedDivisions;
    }

    void ResetHover()
    {
        hoveredTile = null;
        hoveredDivisions = new List<int>();
    }

}

public class SelectedDivisionArgs : EventArgs
{
    public List<int> selectedDivisionIds;
}

public class SelectedTileArgs : EventArgs
{
    public int selectedTileId;
}