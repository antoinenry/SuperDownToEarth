using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LowPolyModel))]
public class LowPolyModelInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LowPolyModel lpm = target as LowPolyModel;

        EditorGUILayout.LabelField("ChangeCount: " + lpm.changeCount);

        if(lpm.referenceModel != null)
        {
            if(GUILayout.Button("Apply changes to reference"))
                lpm.CopyModelTo(lpm.referenceModel);
        }
        else
        {
            if (GUILayout.Button("Send changes"))
                lpm.changeCount++;
        }
    }
}
