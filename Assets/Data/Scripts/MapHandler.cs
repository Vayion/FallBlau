using System;
using System.Collections.Generic;
using UnityEngine;

public class MapHandler : MonoBehaviour
{
    [SerializeField] TileLoader tileLoader;

    private Tile[,] tiles;
    private int sizeX, sizeY;

    /*
     *      0,1     0,3     0,5
     * 0,0     0,2      0,4      
     *      1,1     1,3     1,5
     * 1,0     1,2      1,4
     *      2,1     2,3     2,5
     * 2,0      2,2     2,4
     */


    public void Awake()
    {
        print("Hello?");
    }

    public void generateMap(List<Tile> tiles, int sizeX, int sizeY)
    {
        print("TileCount: "+tiles.Count);
        
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.tiles = new Tile[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                //print(x + ":" + y);
                //print(sizeY * x + y);
                this.tiles[x, y] = tiles[sizeY * x + y];
                this.tiles[x, y].saveCoordinates(x,y);
            }
        }
        
        
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                this.tiles[x,y].setLandmassID(0);
                this.tiles[x,y].setNeighbours(getNeighbours(x, y));
            }
        }

        int id = 1;
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                if (this.tiles[x, y].getLandmassID() == 0&& this.tiles[x,y].GetTerrainType()!=Terrain.TerrainType.water)
                {
                    Debug.Log("Injecting Tile: "+x+","+y+": "+id);
                    injectLandMass(id++, this.tiles[x, y], new HashSet<Tile>(), new HashSet<Tile>());
                }
            }
        }
    }

    public Tile[] getNeighbours(int x, int y)
    {
        List<Tile> neighbours = new List<Tile>();


        if (x > 0)
        {
            neighbours.Add(tiles[x - 1, y]);
        }

        if (x + 1 < sizeX)
        {
            neighbours.Add(tiles[x + 1, y]);
        }

        if (y % 2 == 0)
        {
            if (x + 1 < sizeX && y - 1 >= 0)
            {
                neighbours.Add(tiles[x + 1, y-1]);
            }
            if (x + 1 < sizeX && y + 1 < sizeY)
            {
                neighbours.Add(tiles[x + 1, y+1]);
            }

        }
        else
        {
            if (x > 0 && y - 1 >= 0)
            {
                neighbours.Add(tiles[x - 1, y-1]);
            }
            if (x > 0 && y + 1 < sizeY)
            {
                neighbours.Add(tiles[x - 1, y+1]);
            }
        }

        if (y - 1 >= 0)
        {
            neighbours.Add(tiles[x, y - 1]);
        }

        if (y + 1 < sizeY)
        {
            neighbours.Add(tiles[x, y+1]);
        }
        

        return neighbours.ToArray();
    }

    public void injectLandMass(int id, Tile tile, HashSet<Tile> closed, HashSet<Tile> open)
    {
        if (tile.GetTerrainType() == Terrain.TerrainType.water)
        {
            closed.Add(tile);
            return;
        }
        if (closed.Contains(tile))
            return;
        if (open.Contains(tile))
            return;
        open.Add(tile);
        tile.setLandmassID(id);
        foreach (Tile neighbour in tile.getNeighbours())
        {
            injectLandMass(id, neighbour, closed, open);
        }
        closed.Add(tile);
    }
}
