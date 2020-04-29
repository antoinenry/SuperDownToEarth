using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Circuit))]
public class CircuitEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //if(GUILayout.Button("Update trajectory"))
        {
            Circuit circuit = target as Circuit;
            circuit.UpdateTrajectory();

            EditorUtility.SetDirty(circuit);
        }
    }
}
