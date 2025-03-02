using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonMeshGenerator : MonoBehaviour
{
    public float radius = 1f; // Distance from center to any vertex

    public Mesh hexagonMesh;  // Predefined hexagon mesh
    public GameObject hexagonPrefab;
    public Material hexMaterial;
    public int gridWidth = 10;  // Number of hexagons along the x-axis
    public int gridHeight = 10; // Number of hexagons along the z-axis
    public float hexSize = 1f;  // Size of each hexagon

    private Dictionary<int, GameObject> hexagons = new Dictionary<int, GameObject>();

    private List<GameObject> hexagonPrefabs = new List<GameObject>();

    public NeighborArrangements baseA;

    [SerializeField] private List<NeighborArrangements> arrangements = new List<NeighborArrangements>();

    void Start()
    {
        //StartCoroutine("GenerateHexGrid4");
        //GenerateHexGrid();
    }

    void GenerateHexagonMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        // Generate vertices
        Vector3[] vertices = new Vector3[7]; // 6 corners + center
        vertices[0] = Vector3.zero; // Center

        float angleStep = 60f * Mathf.Deg2Rad; // Hexagon has 60° angles
        for (int i = 0; i < 6; i++)
        {
            float angle = i * angleStep;
            vertices[i + 1] = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
        }

        // Generate triangles
        int[] triangles = new int[18]; // 6 triangles, 3 indices each
        for (int i = 0; i < 6; i++)
        {
            int triangleIndex = i * 3;
            triangles[triangleIndex] = 0; // Center
            triangles[triangleIndex + 1] = i + 1; // Current vertex
            triangles[triangleIndex + 2] = (i + 1) % 6 + 1; // Next vertex (wraps around)
        }

        // Generate UVs
        Vector2[] uvs = new Vector2[7];
        uvs[0] = new Vector2(0.5f, 0.5f); // Center UV
        for (int i = 0; i < 6; i++)
        {
            float angle = i * angleStep;
            uvs[i + 1] = new Vector2(Mathf.Cos(angle) * 0.5f + 0.5f, Mathf.Sin(angle) * 0.5f + 0.5f);
        }

        // Assign to mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
    }

    void GenerateHexGrid2()
    {
        float hexWidth = hexSize * Mathf.Sqrt(3f);
        float hexHeight = hexSize * 2f;
        float rowOffset = hexHeight * 0.75f;

        CombineInstance[] combineInstances = new CombineInstance[gridWidth * gridHeight];
        int index = 0;

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector3 position = new Vector3(
                    x * hexWidth + (y % 2 == 0 ? 0 : hexWidth / 2),  // Offset odd rows
                    0,
                    y * rowOffset
                );

                // Create a matrix for the current hexagon's position
                Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.Euler(-90, 0, 0), Vector3.one * hexSize);

                // Add the mesh to the combine instances
                combineInstances[index] = new CombineInstance
                {
                    mesh = hexagonMesh,
                    transform = matrix
                };

                index++;
            }
        }

        // Combine the meshes into one
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstances, true, true);

        // Assign the combined mesh to a new GameObject
        GameObject hexGrid = new GameObject("HexGrid", typeof(MeshFilter), typeof(MeshRenderer));
        hexGrid.GetComponent<MeshFilter>().mesh = combinedMesh;
        hexGrid.GetComponent<MeshRenderer>().material = hexMaterial;

    }

    void GenerateHexGrid()
    {
        float hexWidth = Mathf.Sqrt(3f) * hexSize;  // Horizontal spacing
        float hexHeight = 2f * hexSize;            // Vertical spacing
        float rowOffset = hexWidth * 0.5f;           // Offset for staggered rows

        int i = 0;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++, i++)
            {
                // Calculate position with staggered row offset
                float posX = x * hexWidth;
                float posZ = z * (hexHeight * 0.75f);

                // Offset every odd row
                if ((z + 1) % 2 == 1)
                {
                    posX += rowOffset;
                }

                // Instantiate the hex tile at calculated position
                Vector3 hexPosition = new Vector3(posX, 0.1f, posZ);
                GameObject hexPrefab = GetHexObjectPrefab(0, out int rotation);
                GameObject hexObj = Instantiate(hexPrefab, hexPosition, Quaternion.Euler(new Vector3(-90, 0, rotation)), transform);
                hexObj.transform.localScale = Vector3.one * hexSize;

                hexagons.Add(i, hexObj);
            }
        }
    }

    public void UpdateGrid(List<TileData> tiles)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            UpdateTile(tiles[i].id, tiles[i].country);
        }
    }

    public void UpdateTile(int tile, int country)
    {
        if(hexagons.TryGetValue(tile, out GameObject hexObj))
        {
            hexagons.Remove(tile);
            Destroy(hexObj);
        }

        CreateHexagon(tile, country);
    }

    private void CreateHexagon(int tile, int country)
    {
        if(country < 0) { return; }

        float hexWidth = Mathf.Sqrt(3f) * hexSize;  // Horizontal spacing
        float hexHeight = 2f * hexSize;            // Vertical spacing
        float rowOffset = hexWidth * 0.5f;           // Offset for staggered rows

        int x = tile / gridWidth;                  // Column index
        int z = tile % gridWidth;                  // Row index

        float posX = x * hexWidth;
        float posZ = z * (hexHeight * 0.75f);   // 0.75 accounts for vertical overlap

        // Offset odd rows
        if ((z + 1) % 2 == 1)
        {
            posX += rowOffset;
        }

        Vector3 hexPosition = new Vector3(posX, 0.1f, posZ);
        GameObject hexPrefab = GetHexObjectPrefab(tile, out int rotation);
        GameObject hexObj = Instantiate(hexPrefab, hexPosition, Quaternion.Euler(new Vector3(-90, 0, rotation)), transform);
        hexObj.transform.localScale = Vector3.one * hexSize;
        hexObj.GetComponent<Renderer>().material.color = CountryLoader.countries[country].GetColor();

        hexagons.Add(tile, hexObj);
    }


    IEnumerator GenerateHexGrid4()
    {
        float hexWidth = Mathf.Sqrt(3f) * hexSize;  // Horizontal spacing
        float hexHeight = 2f * hexSize;            // Vertical spacing
        float rowOffset = hexWidth * 0.5f;           // Offset for staggered rows

        for (int i = 0; i < gridHeight * gridWidth; i++)
        {
            int x = i / gridWidth;                  // Column index
            int z = i % gridWidth;                  // Row index

            float posX = x * hexWidth;
            float posZ = z * (hexHeight * 0.75f);   // 0.75 accounts for vertical overlap

            // Offset odd rows
            if ((z + 1) % 2 == 1)
            {
                posX += rowOffset;
            }

            // Instantiate the hex tile at calculated position
            Vector3 hexPosition = new Vector3(posX, 0.1f, posZ);
            GameObject hexObj = Instantiate(hexagonPrefab, hexPosition, Quaternion.Euler(new Vector3(-90, 0, 0)), transform);
            hexObj.transform.localScale = Vector3.one * hexSize;

            yield return new WaitForSeconds(0.005f);

            hexagons.Add(i, hexObj);
        }

    }

    private GameObject GetHexObjectPrefab(int tile, out int rotation)
    {
        //get neighbors of countries

        List<TileData> tileDatas = new List<TileData>();
        TileData tileData = VisualManager.GetTileData(tile);

        for (int i = 0; i < tileData.neighbors.Length; i++)
        {
            tileDatas.Add(VisualManager.GetTileData(tileData.neighbors[i]));
        }
        
        NeighborArrangements arrangement = CreateArrangementFromNeighborWorldPositions(tileData, tileDatas);

        for (int i = 0; i < arrangements.Count; i++)
        {
            if(ArrangmentIsEqualTo(arrangements[i], arrangement))
            {
                GameObject prefab = arrangements[i].prefab;
                rotation = arrangements[i].rotation;
                return prefab;
            }
        }

        rotation = 0;
        return hexagonPrefab;

    }

    [System.Serializable]
    public class NeighborArrangements
    {
        public bool left;
        public bool upleft;
        public bool upright;
        public bool right;
        public bool downright;
        public bool downleft;

        public GameObject prefab;
        public int rotation;

        public NeighborArrangements(bool[] array)
        {
            left = array[0];
            upleft = array[1];
            upright = array[2];
            right = array[3];
            downright = array[4];
            downleft = array[5];
        }

        public bool[] CreateNeighborArray()
        {
            return new bool[]
            {
                left,
                upleft,
                upright,
                right,
                downright,
                downleft,
            };
        }
    }

    public bool ArrangmentIsEqualTo(NeighborArrangements arrangement1, NeighborArrangements arrangment2)
    {
        if(arrangement1.left != arrangment2.left)
        {
            return false;
        }
        if (arrangement1.upleft != arrangment2.upleft)
        {
            return false;
        }
        if (arrangement1.upright != arrangment2.upright)
        {
            return false;
        }
        if (arrangement1.right != arrangment2.right)
        {
            return false;
        }
        if (arrangement1.downright != arrangment2.downright)
        {
            return false;
        }
        if (arrangement1.downleft != arrangment2.downleft)
        {
            return false;
        }

        return true;
    }

    public NeighborArrangements CreateArrangementFromNeighborWorldPositions(TileData tile, List<TileData> neighbors)
    {
        bool[] neigborArrangements = new bool[6];

        for (int i = 0; i < neigborArrangements.Length; i++)
        {
            neigborArrangements[i] = true;
        }

        for (int i = 0; i < neighbors.Count; i++)
        {
            if (neighbors[i].country == tile.country)
            {
                Vector3 current = VisualManager.GetTileVisualPosition(tile.id);
                Vector3 neighbor = VisualManager.GetTileVisualPosition(neighbors[i].id);

                int neighborArrangementPosition = ArrangementPositionFromVector(current, neighbor);

                neigborArrangements[neighborArrangementPosition] = false;
            }
        }

        NeighborArrangements arragement = new NeighborArrangements(neigborArrangements);
        return arragement;
    }

    public int ArrangementPositionFromVector(Vector3 current, Vector3 neighbor)
    {
        Vector3 difference = neighbor - current;
        difference.Normalize();
        Vector2 flatDifference = new Vector2(difference.x, difference.z);

        //Vector2 leftVector = new Vector2(-2, 0);
        //Vector2 upLeftVector = new Vector2(-0.5f, 0.9f);
        //Vector2 uprightVector = new Vector2(0.5f, 0.9f);
        //Vector2 rightVector = new Vector2(2f, 0f);
        //Vector2 downrightVector = new Vector2(0.5f, -0.9f);
        //Vector2 downleftVector = new Vector2(-0.5f, -0.9f);

        Vector2[] directions = new Vector2[]
        {
            new Vector2(-2, 0),      // Left
            new Vector2(-0.5f, 0.9f), // Up-Left
            new Vector2(0.5f, 0.9f),  // Up-Right
            new Vector2(2f, 0f),      // Right
            new Vector2(0.5f, -0.9f), // Down-Right
            new Vector2(-0.5f, -0.9f) // Down-Left
        };

        float minDistance = float.MaxValue;
        int closestIndex = -1;

        for (int i = 0; i < directions.Length; i++)
        {
            float distance = Vector2.Distance(flatDifference, directions[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;

    }
}
