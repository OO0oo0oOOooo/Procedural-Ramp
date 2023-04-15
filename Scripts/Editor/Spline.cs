using System.Collections.Generic;
using UnityEngine;

// TODO: Auto Calculate Segments from resolution
// TODO: Fix Material Updating Bug
// TODO: URP Support
// TODO: Mesh Shape

[ExecuteAlways]
public class Spline : MonoBehaviour
{
    public List<Vector3> points = new List<Vector3>();

    // Spline variables
    public int segments = 1;
    public int steps = 10;

    // Mesh Data
    public Mesh mesh;
    [SerializeField] private List<Vector3> vertices = new List<Vector3>();
    [SerializeField] private List<int> triangles = new List<int>();
    [SerializeField] private List<Vector2> uvs = new List<Vector2>();

    public Color color = Color.gray;
    static readonly int shPropColor = Shader.PropertyToID("_Color");
    MaterialPropertyBlock mpb;
    MaterialPropertyBlock Mpb
    {
        get {
            if(mpb == null)
                mpb = new MaterialPropertyBlock();
            return mpb;
        }
    }

    public void SetColor()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Mpb.SetColor(shPropColor, color);
        meshRenderer.SetPropertyBlock(Mpb);
    }


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


    // Spline
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


    // Mesh
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
                    vertices.Add(point + Quaternion.LookRotation(tangent, Vector3.up) * MeshData.Verts[i]);
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

        staticRamp.AddComponent<InitalizeStaticMesh>();

        staticRamp.GetComponent<InitalizeStaticMesh>().Setup();

        staticRamp.GetComponent<InitalizeStaticMesh>().mesh.SetVertices(vertices);
        staticRamp.GetComponent<InitalizeStaticMesh>().mesh.triangles = triangles.ToArray();
        staticRamp.GetComponent<InitalizeStaticMesh>().mesh.SetUVs(0, uvs);

        staticRamp.GetComponent<InitalizeStaticMesh>().mesh.RecalculateNormals();

        DestroyImmediate(gameObject);

        // Save Points so it can be converted back
    }


    // Maffs
    public Vector3 GetPointPolynomial(float t, int i)
    {
        return transform.TransformPoint(Bezier.GetPointCubicPolynomial(points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
    }

    public Vector3 GetDirection(float t, int i)
    {
        return transform.TransformPoint(Bezier.GetFirstDerivative(points[i + 0], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
    }
}
