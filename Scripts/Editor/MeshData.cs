using UnityEngine;

public static class MeshData
{
    public static readonly Vector3[] Verts = new Vector3[6]
    {
        new Vector3(0, 1, 0), // 0
        new Vector3(-1, 0, 0), // 1

        new Vector3(-1, 0, 0), // 2
        new Vector3(1, 0, 0), // 3

        new Vector3(1, 0, 0), // 4
        new Vector3(0, 1, 0), // 5
    };
    public static readonly int[,] Tris = new int[3,6]
    {
        {0, 1, 6, 6, 1, 7}, // Left

        {2, 3, 8, 8, 3, 9}, // Bot

        {4, 5, 10, 10, 5, 11} // Right
    };
    public static readonly float[] UV = new float[6]
    {
        0,
        1, 
        0,
        1,
        0, 
        1
    };
}
