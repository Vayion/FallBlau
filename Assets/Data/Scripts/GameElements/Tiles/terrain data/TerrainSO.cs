using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewTerrainData", menuName = "GameData/TerrainData")]
public class TerrainSO : ScriptableObject
{
    public int id;
    public string terrainName;
    public Sprite tileInformationImage;
    public string skibidi;
    public List<Texture> terrainTextures = new List<Texture>();
    public Material terrainMaterial;
}
