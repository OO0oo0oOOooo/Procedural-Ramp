using System.Collections.Generic;
using UnityEngine;

// TODO: Mesh Cap Uvs

[ExecuteAlways]
public class Spline : MonoBehaviour
{
    public List<Vector3> points = new List<Vector3>();

    // Spline variables
    public int segments = 1;
    public int steps = 10;
    public Vector2 scale = new Vector2(1, 1);

    // Mesh Data
    public Mesh mesh;
    [SerializeField] private List<Vector3> vertices = new List<Vector3>();
    [SerializeField] private List<int> triangles = new List<int>();
    [SerializeField] private List<Vector2> uvs = new List<Vector2>();

    void OnEnable()
    {
        if(points.Count < 4)
        {
            InitPoints();
        }

        if(mesh == null)
            InitMesh();

        DrawMesh();
    }

    void OnDisable()
    {
        if(mesh != null)
        {
            DestroyImmediate(mesh);
        }
    }


    public void InitPoints()
    {
        points.Clear();
        for(int i = 0; i < 4; i++)
        {
            points.Add(transform.position + new Vector3(0, 0, (points.Count * 5)));
        }
    }

    public void AddPoints()
    {
        segments++;
        int index = (((segments - 1) * 3) - 3);
        for(int i = 0; i < 3; i++)
        {
            points.Add(GetPointPolynomial(1, index) + GetDirection(1, index).normalized * ((i * 5) + 5));
        }
    }

    public void RemovePoints()
    {
        segments--;
        for(int i = 0; i < 3; i++)
        {
            points.RemoveAt(points.Count - 1);
        }
    }


    #region Mesh
    private void CreateMesh(int segmentCount, int steps)
    {
        int triangleIndex = 0;
        int index = 0;

        for(int currentSegment = 1; currentSegment <= segmentCount; currentSegment++)
        {
            for(int s = 0; s <= steps; s++)
            {
                float t = s / ((float)steps);
                Vector3 point = GetPointPolynomial(t, index);
                Vector3 tangent = GetDirection(t, index).normalized;
                for(int i = 0; i < 6; i++)
                {
                    Vector3 vertOffset = Vector3.Scale(MeshData.Verts[i], scale);
                    vertices.Add(point + Quaternion.LookRotation(tangent, Vector3.up) * vertOffset);

                    uvs.Add(new Vector2(MeshData.UV[i], t));
                }
            }

            for(int s = 0; s <= steps - 1; s++)
            {
                for(int k = 0; k < 3; k++)
                {
                    for(int j = 0; j < 6; j++)
                    {
                        triangles.Add(triangleIndex + MeshData.Tris[k,j]);
                    }
                }
                triangleIndex += 6;
            }
            triangleIndex += 6;
            index += 3;
        }
        triangleIndex -= 6;

        triangles.Add(4);
        triangles.Add(2);
        triangles.Add(0);

        triangles.Add(triangleIndex + 0);
        triangles.Add(triangleIndex + 2);
        triangles.Add(triangleIndex + 4);
    }

    private void AssignMeshData()
    {
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
    }

    public void ClearMesh()
    {
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
    }

    public void DrawMesh()
    {
        if(transform.rotation != Quaternion.identity)
            transform.rotation = Quaternion.identity;

        if(transform.localScale != Vector3.one)
            transform.localScale = new Vector3(1, 1, 1);

        ClearMesh();
        CreateMesh(segments, steps);
        AssignMeshData();
    }

    public void InitMesh()
    {
        if(mesh == null)
        {
            mesh = new Mesh(){ hideFlags = HideFlags.HideAndDontSave};
            GetComponent<MeshFilter>().sharedMesh = mesh;
            GetComponent<MeshCollider>().sharedMesh = mesh;
        }
    }

    public void CreateStaticMesh()
    {
        GameObject staticRamp = new GameObject("Static Procedural Ramp");
        staticRamp.transform.position = transform.position;

        staticRamp.AddComponent<MeshFilter>();
        staticRamp.AddComponent<MeshRenderer>();
        staticRamp.AddComponent<MeshCollider>();

        staticRamp.AddComponent<StaticMeshData>();

        StaticMeshData staticMeshData = staticRamp.GetComponent<StaticMeshData>();

        staticMeshData.points = points;
        staticMeshData.segments = segments;
        staticMeshData.steps = steps;

        staticMeshData.Setup();
        staticMeshData.mesh.SetVertices(vertices);
        staticMeshData.mesh.triangles = triangles.ToArray();
        staticMeshData.mesh.SetUVs(0, uvs);
        staticMeshData.mesh.RecalculateNormals();

        DestroyImmediate(gameObject);
    }
    #endregion

    #region Maffs
    public Vector3 GetPointPolynomial(float t, int i)
    {
        return transform.TransformPoint(Bezier.GetPointCubicPolynomial(points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
    }

    public Vector3 GetDirection(float t, int i)
    {
        return transform.TransformPoint(Bezier.GetFirstDerivative(points[i + 0], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
    }
    #endregion
}
