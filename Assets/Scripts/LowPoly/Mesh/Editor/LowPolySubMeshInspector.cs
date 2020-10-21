using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LowPolySubMesh))]
[CanEditMultipleObjects]
public class LowPolySubMeshInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LowPolySubMesh submeshTarget = target as LowPolySubMesh;
        Color color = EditorGUILayout.ColorField("Mesh color", submeshTarget.meshColor);

        foreach (Object t in targets)
            SetSubMeshColor(t as LowPolySubMesh, color);
    }

    private void SetSubMeshColor(LowPolySubMesh sub, Color col)
    {
        if (col != sub.meshColor)
        {
            sub.meshColor = col;
            sub.OnChange.Invoke();
        }
    }
}
