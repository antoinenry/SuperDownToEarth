using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
/*
[CustomEditor(typeof(CircuitBody))]
public class CircuitBodyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        bool changeCheck = EditorGUI.EndChangeCheck();

        CircuitBody body = target as CircuitBody;
        string moveButtonText = body.invertDirection ? "<<" : ">>";
        if (GUILayout.Button(moveButtonText))
        {
            changeCheck = true;
            body.moveToStep.Value = body.moveToStep + body.CurrentDirection;
        }

        if (changeCheck && !Application.isPlaying)
            body.MoveTo(body.moveToStep, true);
    }
}
*/