using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
[ExecuteInEditMode]
public class MeshGenerator : MonoBehaviour
{
    [Header("Size")]
    public int size = 100;
    public float cellSize = 5f;

    [Header("Layer 1 — Big Hills")]
    public float layer1Scale = 0.005f;
    public float layer1Weight = 0.6f;

    [Header("Layer 2 — Medium Hills")]
    public float layer2Scale = 0.015f;
    public float layer2Weight = 0.3f;

    [Header("Layer 3 — Detail")]
    public float layer3Scale = 0.05f;
    public float layer3Weight = 0.1f;

    [Header("Height")]
    public float heightMultiplier = 40f;

    [Header("Seed")]
    public int seed = 0;
    public bool randomizeSeed = true;

    [Header("Editor")]
    public bool generateInEditor = true;

    private Mesh mesh;

    void Start()
    {
        if (Application.isPlaying && randomizeSeed)
            seed = Random.Range(0, 100000);

        if (Application.isPlaying || generateInEditor)
            GenerateMesh();
    }

    void OnValidate()
    {
        if (!Application.isPlaying && generateInEditor)
            GenerateMesh();
    }

    void GenerateMesh()
    {
        if (size < 1) return;

        int vertsPerLine = size + 1;
        Vector3[] vertices = new Vector3[vertsPerLine * vertsPerLine];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[size * size * 6];

        float halfSize = size * cellSize * 0.5f;
        float seedOffset = seed * 0.01f;

        int i = 0;
        for (int z = 0; z <= size; z++)
        {
            for (int x = 0; x <= size; x++)
            {
                float worldX = x * cellSize - halfSize;
                float worldZ = z * cellSize - halfSize;

                float noise1 = Mathf.PerlinNoise(
                    (worldX * layer1Scale) + seedOffset,
                    (worldZ * layer1Scale) + seedOffset
                );

                float noise2 = Mathf.PerlinNoise(
                    (worldX * layer2Scale) + seedOffset + 200f,
                    (worldZ * layer2Scale) + seedOffset + 200f
                );

                float noise3 = Mathf.PerlinNoise(
                    (worldX * layer3Scale) + seedOffset + 400f,
                    (worldZ * layer3Scale) + seedOffset + 400f
                );

                float combined = noise1 * layer1Weight
                               + noise2 * layer2Weight
                               + noise3 * layer3Weight;

                float height = combined * heightMultiplier;

                vertices[i] = new Vector3(worldX, height, worldZ);
                uvs[i] = new Vector2((float)x / size, (float)z / size);
                i++;
            }
        }

        int tri = 0;
        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                int bl = z * vertsPerLine + x;
                int br = bl + 1;
                int tl = bl + vertsPerLine;
                int tr = tl + 1;

                triangles[tri++] = bl;
                triangles[tri++] = tl;
                triangles[tri++] = tr;

                triangles[tri++] = bl;
                triangles[tri++] = tr;
                triangles[tri++] = br;
            }
        }

        if (mesh == null)
            mesh = new Mesh();

        mesh.name = "ProceduralTerrain";
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}