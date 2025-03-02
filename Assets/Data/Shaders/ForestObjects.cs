using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ForestObjects : MonoBehaviour
{
    public Mesh mesh;
    public Material material;
    public Camera mainCamera;

    private ComputeBuffer argsBuffer;

    [SerializeField] private Mesh tree;
    [SerializeField] private Material treeMaterial;
    [SerializeField] private float randomDisplacement;
    [SerializeField] private int treeAmount;
    [SerializeField] private Vector2 minMaxScale;

    [SerializeField] private List<Mesh> houses = new List<Mesh>();
    [SerializeField] private Material houseMaterial;
    [SerializeField] private float minHouseAmount, maxHouseAmount;
    [SerializeField] private Vector2 minMaxScaleHouse;

    //private void Start()
    //{
    //    argsBuffer = new ComputeBuffer(1, 5*sizeof(uint), ComputeBufferType.IndirectArguments);

    //    uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    //    args[0] = mesh.GetIndexCount(0);
    //    args[1] = 1000000;
    //    args[2] = mesh.GetIndexStart(0);
    //    args[3] = mesh.GetBaseVertex(0);
    //    args[4] = 0;

    //    argsBuffer.SetData(args);
    //}

    //private void Update()
    //{
    //    Bounds bounds = new Bounds(Vector3.zero, new Vector3(300, 100, 300));
    //    Graphics.DrawMeshInstancedIndirect(mesh, 0, material, bounds, argsBuffer);
    //}

    public void StartGeneration(TileLoader tileBehaviourScript)
    {
        Texture2D texture = tileBehaviourScript.GetMapSettings();

        for (int i = 0; i < tileBehaviourScript.tileVisuals.Count; i++)
        {
            TileVisual tile = tileBehaviourScript.tileVisuals[i];

            int randomRotationY = Random.Range(0, 360);

            Terrain.TerrainType tileTerrain = tile.GetTerrainType();

            if (tileTerrain == Terrain.TerrainType.forest)
            {

                Vector3[] randomPoints = SamplePointsInHexagon(tile.GetPosition(), 1, treeAmount);
                CombineInstance[] combine = new CombineInstance[randomPoints.Length];

                for (int hexPoint = 0; hexPoint < randomPoints.Length; hexPoint++)
                {
                    combine[hexPoint].mesh = tree;

                    int randomRotation = Random.Range(0, 360);

                    float randomDisplacementX = Random.Range(-randomDisplacement, randomDisplacement);
                    float randomDisplacementY = Random.Range(-randomDisplacement, randomDisplacement);

                    Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(new Vector3(-90, randomRotation, 0)));  // Apply rotation
                    Matrix4x4 translationMatrix = Matrix4x4.Translate(randomPoints[hexPoint] + new Vector3(randomDisplacementX, 0, randomDisplacementY));  // Apply position
                    Matrix4x4 scaleMatrix = Matrix4x4.Scale(Vector3.one * Random.Range(minMaxScale.x, minMaxScale.y));  // Apply random scale

                    combine[hexPoint].transform = translationMatrix * rotationMatrix * scaleMatrix;
                }

                Mesh combinedMesh = new Mesh();
                combinedMesh.CombineMeshes(combine, true, true);

                GameObject chunkParent = new GameObject("tree object chunk");
                chunkParent.transform.parent = transform;
                MeshFilter chunkMeshFilter = chunkParent.AddComponent<MeshFilter>();
                MeshRenderer chunkRenderer = chunkParent.AddComponent<MeshRenderer>();

                chunkMeshFilter.sharedMesh = combinedMesh;
                chunkRenderer.material = treeMaterial;

            }
            else if (tileTerrain == Terrain.TerrainType.plains)
            {
                int randomHouseAmount = (int)Random.Range(minHouseAmount, maxHouseAmount);

                Vector3[] randomPoints = SamplePointsInHexagon(tile.GetPosition(), 1, randomHouseAmount);
                CombineInstance[] combine = new CombineInstance[randomPoints.Length];

                for (int hexPoint = 0; hexPoint < randomPoints.Length; hexPoint++)
                {
                    int randomIndex = Random.Range(0, houses.Count);
                    combine[hexPoint].mesh = houses[randomIndex];

                    int randomRotation = Random.Range(0, 360);

                    float randomDisplacementX = Random.Range(-randomDisplacement, randomDisplacement);
                    float randomDisplacementY = Random.Range(-randomDisplacement, randomDisplacement);

                    Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(new Vector3(-90, randomRotation, 0)));  // Apply rotation
                    Matrix4x4 translationMatrix = Matrix4x4.Translate(randomPoints[hexPoint] + new Vector3(randomDisplacementX, 0, randomDisplacementY));  // Apply position
                    Matrix4x4 scaleMatrix = Matrix4x4.Scale(Vector3.one * Random.Range(minMaxScaleHouse.x, minMaxScaleHouse.y));  // Apply random scale

                    combine[hexPoint].transform = translationMatrix * rotationMatrix * scaleMatrix;
                }

                Mesh combinedHouseMesh = new Mesh();
                combinedHouseMesh.CombineMeshes(combine, true, true);

                GameObject chunkParent = new GameObject("house object chunk");
                chunkParent.transform.parent = transform;
                MeshFilter chunkMeshFilter = chunkParent.AddComponent<MeshFilter>();
                MeshRenderer chunkRenderer = chunkParent.AddComponent<MeshRenderer>();

                chunkMeshFilter.sharedMesh = combinedHouseMesh;
                chunkRenderer.material = houseMaterial;
            }
            
        }
    }


    private int ColorToHex(Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255f);
        int g = Mathf.RoundToInt(color.g * 255f);
        int b = Mathf.RoundToInt(color.b * 255f);
        return (r << 16) | (g << 8) | b;
    }

    // Generate random points within a hexagon
    public static Vector3[] SamplePointsInHexagon(Vector3 center, float radius, int numPoints)
    {
        Vector3[] vertices = GenerateHexagonVertices(center, radius);
        Vector3[] points = new Vector3[numPoints];

        for (int i = 0; i < numPoints; i++)
        {
            // Randomly pick a triangle
            int triangleIndex = Random.Range(0, 6);
            Vector3 p1 = center;
            Vector3 p2 = vertices[triangleIndex];
            Vector3 p3 = vertices[(triangleIndex + 1) % 6];

            // Generate a random point within the selected triangle
            points[i] = SamplePointInTriangle(p1, p2, p3);
        }

        return points;
    }

    // Generate vertices for a regular hexagon
    private static Vector3[] GenerateHexagonVertices(Vector3 center, float radius)
    {
        Vector3[] vertices = new Vector3[6];
        float rotationAngle = 30f; // Rotate by 30 degrees
        Quaternion rotation = Quaternion.Euler(0, rotationAngle, 0); // Create a rotation quaternion for the y-axis

        for (int i = 0; i < 6; i++)
        {
            float angle = Mathf.Deg2Rad * (60 * i);  // Calculate the angle for each hexagon vertex
            Vector3 vertex = new Vector3(radius * Mathf.Cos(angle), 0f, radius * Mathf.Sin(angle));
            // Apply the rotation to each vertex
            vertices[i] = rotation * vertex + center;  // Rotate and then translate to the center of the hexagon
        }
        return vertices;
    }

    // Sample a random point in a triangle using barycentric coordinates
    private static Vector3 SamplePointInTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float r1 = Random.value;
        float r2 = Random.value;

        // Ensure uniform distribution
        if (r1 + r2 > 1f)
        {
            r1 = 1f - r1;
            r2 = 1f - r2;
        }

        return p1 + r1 * (p2 - p1) + r2 * (p3 - p1);
    }
}
