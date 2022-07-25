using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spline))]
public class BezierEditor : Editor
{
    Spline spline;

    SerializedObject so;
    SerializedProperty propPoints;
    SerializedProperty propSegments;
    SerializedProperty propSteps;
    SerializedProperty propColor;


    [MenuItem("Tools/Procedural Ramp")]
    public static void SpawnRamp()
    {
        var proceduralRampPrefab = Resources.Load("Procedural Ramp");
        Instantiate(proceduralRampPrefab, Vector3.zero, Quaternion.identity);
    }


    void OnEnable()
    {
        so = serializedObject;
        propPoints = so.FindProperty("points");
        propSegments = so.FindProperty("segments");
        propSteps = so.FindProperty("steps");
        propColor = so.FindProperty("color");

        Undo.undoRedoPerformed += UndoCallback;
    }

    void OnDisable()
    {
        Undo.undoRedoPerformed -= UndoCallback;
    }

    public override void OnInspectorGUI()
    {
        spline = target as Spline;
        // base.OnInspectorGUI();

        so.Update();
        
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(propPoints);
        EditorGUILayout.PropertyField(propSegments);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.PropertyField(propSteps);
        EditorGUILayout.PropertyField(propColor);
        if (so.ApplyModifiedProperties())
        {
            if(spline.mesh == null)
                return;

            spline.DrawMesh();
            spline.SetColor();
        }


        GUILayout.BeginHorizontal();

        Undo.RecordObject(spline, "Add Curve");
        if(GUILayout.Button("Add Curve"))
        {
            spline.AddPoints();
            spline.DrawMesh();
        }

        Undo.RecordObject(spline, "Remove Curve");
        if(GUILayout.Button("Remove Curve"))
        {
            if(spline.segments <= 1)
                return;

            spline.RemovePoints();
            spline.DrawMesh();
        }

        GUILayout.EndHorizontal();


        if(GUILayout.Button("Create Static Mesh"))
        {
            spline.CreateStaticMesh();
        }
    }

    private void OnSceneGUI ()
    {
        spline = target as Spline;

        for(int s = 0; s < spline.segments; s++)
        {
            Handles.color = Color.gray;
            Handles.DrawLine(spline.transform.TransformPoint(spline.points[(s*3) + 0]), spline.transform.TransformPoint(spline.points[(s*3) + 1]));
            Handles.DrawLine(spline.transform.TransformPoint(spline.points[(s*3) + 2]), spline.transform.TransformPoint(spline.points[(s*3) + 3]));
        }

        for(int i = 0; i < spline.points.Count; i++)
        {
            spline.points[i] = ControlPointGUI(i);
        }
    }


    Vector3 snap = Vector3.one * 0.5f;
    private Vector3 ControlPointGUI(int i)
    {
        Handles.color = Color.white;
        Vector3 point = spline.transform.TransformPoint(spline.points[i]);
        float size = HandleUtility.GetHandleSize(point) * 0.1f;

        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(spline, "Move Point");
        point = Handles.FreeMoveHandle(point, Quaternion.identity, size, snap, Handles.CubeHandleCap);
        if (EditorGUI.EndChangeCheck()) {
            AdjustControlPointTangents(i, spline.transform.InverseTransformPoint(point));
            spline.DrawMesh();
        }
        return spline.transform.InverseTransformPoint(point);
    }

    void AdjustControlPointTangents(int i, Vector3 point)
    {
        if(i == 1 || i == spline.points.Count - 2)
            return;

        int knot = ((i+1)/3) * 3;
        Vector3 d = point - spline.points[i];
        
        
        if(i == knot)
        {   
            if(i == 0)
            {
                spline.points[knot + 1] += d;
                return;
            }
            else if(i == spline.points.Count - 1)
            {
                spline.points[knot - 1] += d;
                return;
            }
            else
            {
                spline.points[knot - 1] += d;
                spline.points[knot + 1] += d;
                return;
            }
        }
        
        Vector3 middle = spline.points[knot];

        if (i < knot)
        {
            spline.points[knot + 1] = middle + (middle - (spline.points[knot - 1]));
        }
        else
        {
            spline.points[knot - 1] = middle + (middle - (spline.points[knot + 1]));
        }
    }

    void UndoCallback()
    {
        spline.DrawMesh();
        spline.SetColor();
    }
}
