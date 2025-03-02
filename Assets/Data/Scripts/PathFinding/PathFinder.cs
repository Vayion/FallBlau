using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface PathFinder
{
    public static Dictionary<Country, PathFindingCache> caches = new Dictionary<Country, PathFindingCache>();
    
    public static LinkedList<Tile> FindPath(Tile start, Tile end, Country country)
    {
        if (!caches.ContainsKey(country))
        {
            caches[country] = new PathFindingCache();
        }

        LinkedList<Tile> cachedValue = caches[country].FindPath(start, end);

        if (cachedValue != null)
        {
            return cachedValue;
        }

        if (start.getLandmassID() != end.getLandmassID())
        {
            return new LinkedList<Tile>();
        }

        /*
        LinkedList<Tile> temp = new LinkedList<Tile>();
        Tile[] tempp2 = start.getNeighbours();
        foreach (Tile temp2 in tempp2)
            temp.AddLast(temp2);
        temp.AddLast(end);


        return temp;

        */
        LinkedList<Tile> output = new LinkedList<Tile>();
        output.AddFirst(end);
        
        LinkedList<Tile> path = new LinkedList<Tile>();

        //Priority Queueof tiles and their f-costs (Sorted List requires two args but will always receive the same one twice)
        Dictionary<Tile, float> fcosts = new Dictionary<Tile, float>();
        SortedSet<Tile> open = new  SortedSet<Tile>(Comparer<Tile>.Create((x, y) =>
        {
            float x1 = fcosts[x];
            float y1 = fcosts[y];

            if (x1 > y1)
                return 1;
            if (x1 < y1)
                return -1;
            if (x.x > y.x)
                return 1;
            if (x.x < y.x)
                return -1;
            if (x.y > y.y)
                return 1;
            if (x.y < y.y)
                return -1;
            return 0;
            
        }));
        
        
        // Closed set to track visited nodes
        HashSet<Tile> closed = new HashSet<Tile>();

        // G-Cost
        Dictionary<Tile, float> gCosts = new Dictionary<Tile, float>();
        Dictionary<Tile, Tile> parents = new Dictionary<Tile, Tile>();

        gCosts[start] = 0f;
        fcosts[start] = 0f;
        open.Add(start);

        while(open.Count > 0){
            Tile current = open.First();
            //Debug.Log("Current Tile: "+current.x+", "+current.y);
            Tile[] neighbours = current.getNeighbours();
            if (current == end)
            {
                
                //Debug.Log("Found path");
                Tile tmp = end;
                while (tmp != start)
                {
                    path.AddFirst(tmp);
                    tmp = parents[tmp];
                }
                caches[country].addPath(start, end, path);
                return path;
            }
            
            open.Remove(current);
            closed.Add(current);
            
            foreach (Tile neighbour in neighbours)
            {

                if (neighbour.GetTerrainType() == Terrain.TerrainType.water)
                {
                    closed.Add(neighbour);
                    continue;
                }
                if (closed.Contains(neighbour))
                {
                    continue;
                }
                float newGCost = gCosts[current] + getTravelCost(current, neighbour);
                if (fcosts.ContainsKey(neighbour)&& newGCost > gCosts[neighbour])
                {
                    continue;
                }

                if(fcosts.ContainsKey(neighbour))
                    open.Remove(neighbour);
                gCosts[neighbour] = newGCost;
                fcosts[neighbour] = newGCost + GetHeuristic(neighbour, end);
                //Debug.Log("fcost of Tile: "+neighbour.x+", "+neighbour.y+": "+fcosts[neighbour]);
                parents[neighbour] = current;
                open.Add(neighbour);
            }
        }
        
        //Debug.Log("No path found");
        return path;
    }

    private static LinkedList<Tile> ReconstructPath(Dictionary<Tile, Tile> parents, Tile current)
    {
        LinkedList<Tile> path = new LinkedList<Tile>();

        while (current != null)
        {
            path.AddFirst(current);
            parents.TryGetValue(current, out current);
        }

        return path;
    }

    private static float getTravelCost(Tile current, Tile next)
    {
        if (next.GetTerrainType() == Terrain.TerrainType.mountains)
        {
            return 4;
        }
        return 1;
    }

    private static float GetHeuristic(Tile a, Tile b)
    {
        Vector3 aPos = a.GetWorldPosition();
        Vector3 bPos = b.GetWorldPosition();

        return Mathf.Abs(aPos.x - bPos.x) + Mathf.Abs(aPos.z - bPos.z);
    }
    
}
