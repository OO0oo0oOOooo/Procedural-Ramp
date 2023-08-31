using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class StaticMeshData : MonoBehaviour
{
    [HideInInspector] public Mesh mesh;

    [HideInInspector] public List<Vector3> points;
    [HideInInspector] public int segments = 1;
    [HideInInspector] public int steps = 10;

    public void Setup()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
        GetComponent<MeshRenderer>().sharedMaterial = Resources.Load<Material>("RampMat");
    }

    [ContextMenu("Convert Spline")]
    public void CreateSpline()
    {
        GameObject customObject = new GameObject("Procedural Ramp");
        customObject.transform.position = transform.position;
        // customObject.transform.rotation = transform.rotation;

        customObject.AddComponent<MeshRenderer>();
        customObject.AddComponent<MeshFilter>();
        customObject.AddComponent<MeshCollider>();
        customObject.AddComponent<Spline>();

        Material material = Resources.Load<Material>("RampMat");
        customObject.GetComponent<MeshRenderer>().material = material;

        Spline spline = customObject.GetComponent<Spline>();
        spline.points = points;
        spline.segments = segments;
        spline.steps = steps;

        spline.DrawMesh();

        DestroyImmediate(gameObject);
    }
}
