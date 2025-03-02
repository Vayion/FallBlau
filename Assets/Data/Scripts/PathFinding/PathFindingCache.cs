
using System.Collections.Generic;
using UnityEngine;

public class PathFindingCache
{

    private static int MAX_SIZE = 15;
    private int index = 0;
    KeyValuePair<Tile, Tile>[] buffer = new KeyValuePair<Tile, Tile>[MAX_SIZE];
    Dictionary<KeyValuePair<Tile, Tile>, LinkedList<Tile>> cache = new Dictionary<KeyValuePair<Tile, Tile>, LinkedList<Tile>>();

    public LinkedList<Tile> FindPath(Tile start, Tile end)
    {
        KeyValuePair<Tile, Tile> temp = new KeyValuePair<Tile, Tile>(start, end);
        if (cache.ContainsKey(temp))
        {
            return cache[temp];
        }

        return null;
    }

    public void addPath(Tile start, Tile end, LinkedList<Tile> path)
    {
        KeyValuePair<Tile, Tile> temp = buffer[index];
        cache.Remove(temp);
        buffer[index] = new KeyValuePair<Tile, Tile>(start, end);
        cache[buffer[index]] = path;
        index = (index + 1) % MAX_SIZE;
    }
}
