using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Country
{
    public int id;
    private HashSet<Tile> tiles = new HashSet<Tile>();
    private Texture2D flag;
    private Color color;
    private String name;
    private String tag;

    public Country(Texture2D flag, Color color, String name, String tag, int id)
    {
        this.flag = flag;
        this.color = color;
        this.name = name;
        this.tag = tag;
        tiles = new HashSet<Tile>();
        this.id = id;
    }

    public String GetName()
    {
        return name;
    }

    public List<Tile> GetTiles()
    {
        return tiles.ToList();
    }

    public Color GetColor()
    {
        return color;
    }

    public void AddTile(Tile tile)
    {
        if (!tiles.Contains(tile))
        {
            tiles.Add(tile);
        }
    }

    public Texture2D GetFlag()
    {
        return flag;
    }

    public bool isHostileTo(Country other)
    {
        
        //TODO: add Faction logic
        return !this.Equals(other);
    }

    public bool isFriendlyTo(Country other)
    {
        //TODO: add Faction logic
        return this.Equals(other);
    }


}
