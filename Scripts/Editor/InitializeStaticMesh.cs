using UnityEngine;

[ExecuteAlways]
public class InitalizeStaticMesh : MonoBehaviour
{
    public Mesh mesh;

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

    public void Setup()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
        GetComponent<MeshRenderer>().sharedMaterial = Resources.Load<Material>("Material");
    }

    void OnValidate()
    {
        SetColor();
    }
}
