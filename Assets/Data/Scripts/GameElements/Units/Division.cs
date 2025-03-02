using System;
using System.Collections.Generic;
using System.Linq;
using Data.Scripts.GameElements.Units;
using UnityEngine;

public class Division : LandCombatStats
{
    public int id;

    private Tile target;
    private Tile current;
    private LinkedList<Tile> queue = new LinkedList<Tile>();
    private float progress;
    private Country country;

    private bool retreating;
    
    private LandBattle battle;


    public Division()
    {
        GameTime.OnNewHour += FixedUpdate;

        
        //TODO: replace with actual values
        org = 50;
        maxOrg = 100;
        hp = 50;
        maxHp = 100;
    }

    public void FixedUpdate(object sender, GameTime.OnNewHourEvent e)
    {
        // server logic

        if (org <= 0)
        {
            retreat();
        }

        //Get new target from queue
        if (target == null)
        {
            if (queue.Count > 0)
            {
                progress = Defines.tileSize;
                target = queue.First();
                
                //Enter combat if tile is hostile
                if (target.GetCountry().isHostileTo(country))
                {
                    target.joinBattleAttacker(this);
                }
                
                queue.RemoveFirst();
            }
        }

        if (target != null)
        {
            
            progress -= Defines.divisionBaseSpeed;
            
            // Enter new Tile (upon reaching it and no (blocking) combat happening
            if(progress <= 0 && target.canEnter(this))
            {
                MoveToTile(target);
                target = null;
                retreating = false;
            }
            
        }

        TheGameManager.SendDivisionUpdate(this);

    }

    public void setTarget(Tile tile)
    {
        //TODO check if valid

        if (retreating)
        {
            //cant change while retreating
            return;
        }

        if (!tile.GetCountry().isFriendlyTo(country)&&!tile.GetCountry().isHostileTo(country))
        {
            //The Tile cannot be entered (neither friendly nor opponent)
            return;
        }

        LinkedList<Tile> newTarget = PathFinder.FindPath(current, tile, country);
        if (newTarget.Count == 0)
        {
            return;
        }

        if(current == newTarget.First()) { return; }
        
        progress = Defines.tileSize;
        queue = new LinkedList<Tile>(newTarget);
        
        //overwrite old target, leave possible battles
        if (target != null)
        {
            target.leaveBattleAttacker(this);
        }

        target = null;
    }

    public Tile GetCurrentTile()
    {
        return current;
    }

    public Tile GetTargetTile()
    {
        if(target == null) { return current; }

        return target;
    }

    public void MoveToTile(Tile tile)
    {
        if ((current != null)) { current.divisionExitTile(this); }
        current = tile;
        target = null;
        current.divisionEnterTile(this);
        if (current.GetCountry().isHostileTo(country))
        {
            current.overTakeTile(country);
        }

        TheGameManager.SendDivisionUpdate(this);
    }

    public void LoadData(int id_, Tile target_, Tile current_, float progress_, Country country_)
    {
        id = id_;
        target = target_;
        current = current_;
        progress = progress_;
        country = country_;
    }

    public float GetProgressPercent()
    {
        return progress / Defines.tileSize;
    }

    public float GetDistanceFromTarget()
    {
        return (target.GetWorldPosition() - current.GetWorldPosition()).magnitude;
    }

    public Vector3 GetTargetPosition()
    {
        return target.GetWorldPosition();
    }

    public Vector3 GetPosition()
    {
        return current.GetWorldPosition();
    }

    public Country GetCountry()
    {
        return country;
    }

    public double getOrgPercent()
    {
        return org/maxOrg>1?1:org/maxOrg;
    }

    public double getHealthPercent()
    {
        return hp / maxHp > 1 ? 1 : hp / maxHp;
    }

    public void SetCountry(Country country_)
    {
        country = country_;
    }

    public void retreat()
    {
        current.divisionExitTile(this);
        org = 50;
        
        if (current.getLandBattle() == null)
        {
            //Attacking combat, dont need to retreat, just stop attacking
            target = null;
            queue = null;
        }

        Tile[] neighbours = current.getNeighbours();

        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i].GetCountry().isFriendlyTo(country))
            {
                setTarget(neighbours[i]);
                retreating = true;
                return;
            }
        }
        
        //No Valid target found --> Kill Division
        despawnDivision();
    }

    public void despawnDivision()
    {
        Debug.Log("Despawn Division");
        GameTime.OnNewHour -= FixedUpdate;
        TheGameManager.DespawnDivision(this);
        if (current != null)
        {
            current.divisionExitTile(this);
        }

        if (target != null)
        {
            target.leaveBattleAttacker(this);
        }
    }
}
