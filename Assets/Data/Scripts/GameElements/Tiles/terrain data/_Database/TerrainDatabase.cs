using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Terrain Database", menuName = "Databases/Terrain Database")]
public class TerrainDatabase : ScriptableObject, ISerializationCallbackReceiver
{
    public TerrainSO[] Terrains;
    public Dictionary<int, TerrainSO> TerrainDictionary = new Dictionary<int, TerrainSO>();

    public void OnAfterDeserialize()
    {
        for (int i = 0; i < Terrains.Length; i++)
        {
            Terrains[i].id = i;
            TerrainDictionary.Add(i, Terrains[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        TerrainDictionary = new Dictionary<int, TerrainSO>();
    }
}
