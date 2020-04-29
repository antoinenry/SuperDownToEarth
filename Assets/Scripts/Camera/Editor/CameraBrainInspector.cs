using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraBrain))]
public class CameraBrainInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Apply default settings"))
        {
            CameraBrain cb = target as CameraBrain;
            cb.SetAllSettingToDefault();
            cb.ApplyCurrentSettings();
        }
    }
}
