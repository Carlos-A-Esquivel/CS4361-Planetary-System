using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RingMesh : MonoBehaviour
{
    [SerializeField] private float innerRadius = 1.2f;
    [SerializeField] private float outerRadius = 2.2f;
    [SerializeField] private int segments = 128;

    void Start()
    {
        GenerateRing();
    }

    void GenerateRing()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Ring";

        // Two vertices per segment: one on the inner edge, one on the outer edge
        int vertexCount = (segments + 1) * 2;
        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uvs = new Vector2[vertexCount];
        int[] triangles = new int[segments * 6];

        float angleStep = (2f * Mathf.PI) / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            int v = i * 2;

            // Flat ring lying in the XZ plane
            vertices[v]     = new Vector3(cos * innerRadius, 0f, sin * innerRadius);
            vertices[v + 1] = new Vector3(cos * outerRadius, 0f, sin * outerRadius);

            // Polar UVs: U runs 0 (inner) to 1 (outer), so the texture strip
            // maps radially outward. V runs around the circumference.
            float vCoord = (float)i / segments;
            uvs[v]     = new Vector2(0f, vCoord);
            uvs[v + 1] = new Vector2(1f, vCoord);
        }

        for (int i = 0; i < segments; i++)
        {
            int v = i * 2;
            int t = i * 6;

            triangles[t]     = v;
            triangles[t + 1] = v + 1;
            triangles[t + 2] = v + 2;

            triangles[t + 3] = v + 1;
            triangles[t + 4] = v + 3;
            triangles[t + 5] = v + 2;
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}