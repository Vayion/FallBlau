using System.Collections.Generic;
using Data.Scripts.GameElements.Units;
using UnityEngine;

public class Tile
{
    public int id { get; private set; }

    private Country country;

    private int landmassID;
    public Terrain.TerrainType terrainType;
    private Vector3 worldPosition;

    private LandBattle battle;
    
    private HashSet<Division> divisions = new HashSet<Division>();
    
    private Tile[] neighbours;

    public int x, y;

    public void Setup(int id_, Terrain.TerrainType terrainType_, Vector3 worldPosition_, Country country_)
    {
        id = id_;
        terrainType = terrainType_;
        worldPosition = worldPosition_;
        SetCountry(country_);
    }

    public int getLandmassID() { return landmassID; }
    public void setLandmassID(int value) { landmassID = value; }
    public void saveCoordinates(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector3 GetWorldPosition()
    {
        return worldPosition;
    }
    
    public void setNeighbours(Tile[] neighbours)
    {
        this.neighbours = neighbours;
    }

    public Tile[] getNeighbours()
    {
        return neighbours;
    }
    public Terrain.TerrainType GetTerrainType()
    {
        return terrainType;
    }

    public void SetCountry(Country country_)
    {
        country = country_;
    }

    public Country GetCountry()
    {
        return country;
    }

    public LandBattle getLandBattle()
    {
        return battle;
    }

    private LandBattle startNewBattle(Division division)
    {
        battle = new LandBattle(this, new HashSet<Division> { division });
        return battle;
    }

    public void divisionEnterTile(Division div)
    {
        divisions.Add(div);
    }

    public void divisionExitTile(Division div)
    {
        divisions.Remove(div);
    }

    public void joinBattleAttacker(Division div)
    {
        if (battle != null)
        {

            battle.GetAttackers().Add(div);
        }
        else
        {
            battle = startNewBattle(div);
        }
    }

    
    public void leaveBattleAttacker(Division div)
    {
        if (battle != null)
        {
            battle.GetAttackers().Remove(div);
        }
    }

    public HashSet<Division> getDivisions()
    {
        return divisions;
    }

    public void stopBattle()
    {
        battle = null;
    }

    public bool canEnter(Division div)
    {
        if (div.GetCountry().isFriendlyTo(country))
        {
            return true;
        }
        else
        {
            return battle == null || battle.canEnter();
        }
    }

    public void overTakeTile(Country country)
    {
        if (battle != null)
        {
            battle.stopBattle();
        }

        this.country = country;
        TheGameManager.SendTileData(this);
        //TODO: properly handle battle end
    }
}
