using UnityEngine;
using System.Collections.Generic;

// Generates its own high-resolution sphere (an "icosphere") and displaces it
// with noise that goes both outward (mountains) and inward (craters/basins).
// This replaces the low-poly default sphere entirely, so detail actually shows up.

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MarsTerrainDisplacer : MonoBehaviour
{
    [Header("Sphere detail")]
    [Tooltip("Higher = smoother, more detailed, but slower to generate. 3-5 is a good range.")]
    [Range(1, 6)]
    public int subdivisions = 4;

    [Header("Terrain shape")]
    [Tooltip("How tall mountains / how deep craters get, relative to sphere radius.")]
    public float heightScale = 0.08f;

    [Tooltip("Bigger = wider, smoother features. Smaller = tighter, more frequent bumps.")]
    public float noiseScale = 2.0f;

    [Tooltip("Change this to get a different random terrain layout.")]
    public int seed = 0;

    [Header("Behavior")]
    public bool deformOnStart = true;

    private Mesh _mesh;
    private Vector3[] _baseVertices; // clean, undeformed sphere positions

    void Start()
    {
        if (deformOnStart)
        {
            GenerateAndDeform();
        }
    }

    [ContextMenu("Generate + Deform Now")]
    public void GenerateAndDeform()
    {
        GenerateIcosphere();
        Deform();
    }

    [ContextMenu("Deform Now (keep current sphere)")]
    public void Deform()
    {
        if (_baseVertices == null)
        {
            GenerateIcosphere();
        }

        Vector3[] vertices = new Vector3[_baseVertices.Length];
        System.Random rng = new System.Random(seed);
        Vector3 offset = new Vector3(rng.Next(-1000, 1000), rng.Next(-1000, 1000), rng.Next(-1000, 1000));

        for (int i = 0; i < _baseVertices.Length; i++)
        {
            Vector3 direction = _baseVertices[i].normalized;

            float noise = Perlin3D(
                (direction.x + offset.x) * noiseScale,
                (direction.y + offset.y) * noiseScale,
                (direction.z + offset.z) * noiseScale
            );

            // Center noise around 0 so we get BOTH mountains (positive) and craters (negative),
            // instead of only ever pushing outward.
            float centered = (noise - 0.5f) * 2f; // now roughly -1..1
            float displacement = centered * heightScale;

            vertices[i] = _baseVertices[i] + direction * displacement;
        }

        _mesh.vertices = vertices;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
    }

    private float Perlin3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float yz = Mathf.PerlinNoise(y, z);
        float xz = Mathf.PerlinNoise(x, z);
        float yx = Mathf.PerlinNoise(y, x);
        float zy = Mathf.PerlinNoise(z, y);
        float zx = Mathf.PerlinNoise(z, x);
        return (xy + yz + xz + yx + zy + zx) / 6f;
    }

    // --- Icosphere generation: builds a sphere out of an icosahedron, subdivided
    // repeatedly so triangles get smaller/denser, giving you real detail to displace. ---

    private void GenerateIcosphere()
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var midpointCache = new Dictionary<long, int>();

        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        AddVertex(vertices, new Vector3(-1, t, 0));
        AddVertex(vertices, new Vector3(1, t, 0));
        AddVertex(vertices, new Vector3(-1, -t, 0));
        AddVertex(vertices, new Vector3(1, -t, 0));
        AddVertex(vertices, new Vector3(0, -1, t));
        AddVertex(vertices, new Vector3(0, 1, t));
        AddVertex(vertices, new Vector3(0, -1, -t));
        AddVertex(vertices, new Vector3(0, 1, -t));
        AddVertex(vertices, new Vector3(t, 0, -1));
        AddVertex(vertices, new Vector3(t, 0, 1));
        AddVertex(vertices, new Vector3(-t, 0, -1));
        AddVertex(vertices, new Vector3(-t, 0, 1));

        int[,] faces = {
            {0,11,5},{0,5,1},{0,1,7},{0,7,10},{0,10,11},
            {1,5,9},{5,11,4},{11,10,2},{10,7,6},{7,1,8},
            {3,9,4},{3,4,2},{3,2,6},{3,6,8},{3,8,9},
            {4,9,5},{2,4,11},{6,2,10},{8,6,7},{9,8,1}
        };

        for (int i = 0; i < faces.GetLength(0); i++)
        {
            triangles.Add(faces[i, 0]);
            triangles.Add(faces[i, 1]);
            triangles.Add(faces[i, 2]);
        }

        for (int s = 0; s < subdivisions; s++)
        {
            var newTriangles = new List<int>();
            for (int i = 0; i < triangles.Count; i += 3)
            {
                int a = triangles[i];
                int b = triangles[i + 1];
                int c = triangles[i + 2];

                int ab = GetMidpoint(a, b, vertices, midpointCache);
                int bc = GetMidpoint(b, c, vertices, midpointCache);
                int ca = GetMidpoint(c, a, vertices, midpointCache);

                newTriangles.AddRange(new[] { a, ab, ca, b, bc, ab, c, ca, bc, ab, bc, ca });
            }
            triangles = newTriangles;
            midpointCache.Clear();
        }

        // Spherical (equirectangular) UVs so a texture can wrap around the sphere correctly.
        var uvs = new Vector2[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 dir = vertices[i].normalized;
            float u = 0.5f + Mathf.Atan2(dir.z, dir.x) / (2f * Mathf.PI);
            float v = 0.5f - Mathf.Asin(dir.y) / Mathf.PI;
            uvs[i] = new Vector2(u, v);
        }

        _mesh = new Mesh();
        _mesh.indexFormat = vertices.Count > 65000
            ? UnityEngine.Rendering.IndexFormat.UInt32
            : UnityEngine.Rendering.IndexFormat.UInt16;
        _mesh.vertices = vertices.ToArray();
        _mesh.triangles = triangles.ToArray();
        _mesh.uv = uvs;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = _mesh;
        _baseVertices = vertices.ToArray();
    }

    private int AddVertex(List<Vector3> vertices, Vector3 p)
    {
        vertices.Add(p.normalized * 0.5f); // radius 0.5 to match Unity's default sphere size
        return vertices.Count - 1;
    }

    private int GetMidpoint(int a, int b, List<Vector3> vertices, Dictionary<long, int> cache)
    {
        long smaller = Mathf.Min(a, b);
        long larger = Mathf.Max(a, b);
        long key = (smaller << 32) + larger;

        if (cache.TryGetValue(key, out int existing))
        {
            return existing;
        }

        Vector3 midpoint = (vertices[a] + vertices[b]) / 2f;
        int index = AddVertex(vertices, midpoint);
        cache[key] = index;
        return index;
    }
}