using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Data.Scripts.GameElements.Units
{
    public class LandBattle
    {
        private Tile place;
        private HashSet<Division> attackers;
        private bool stop;
        private int progress;

        public LandBattle(Tile place, HashSet<Division> attackers)
        {
            Debug.Log("Starting LandBattle");
            
            GameTime.OnNewHour += FixedUpdate;
            
            this.place = place;
            this.attackers = attackers;
            TheGameManager.CreateLandBattle(this);
        }
        
        
        public void FixedUpdate(object sender, GameTime.OnNewHourEvent e)
        {
            if (attackers.Count == 0)
            {
                stopBattle();
                return;
            }

            double average = 0d;
            foreach (Division defender in place.getDivisions())
            {
                defender.changeOrg(-1);
                average += defender.getOrgPercent();
            }
            if(!stop)
            {
                TheGameManager.LandBattleUpdate(this);
            }
            
            average /= place.getDivisions().Count();
            progress = 100-(int)(average * 100);


        }

        public int getBattleProgress()
        {
            return progress;
        }

        public Tile GetTile()
        {
            return place;
        }

        public HashSet<Division> GetAttackers()
        {
            return attackers;
        }

        public bool canEnter()
        {
            return place.getDivisions().Count == 0;
        }

        public void stopBattle()
        {
            
            Debug.Log("Stopping LandBattle");
            place.stopBattle();
            GameTime.OnNewHour -= FixedUpdate;
            TheGameManager.RemoveLandBattle(this);
            stop = true;
        }
    }
}