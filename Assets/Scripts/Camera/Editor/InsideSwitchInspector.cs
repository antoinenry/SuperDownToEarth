using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InsideSwitch))]
public class InsideSwitchInspector : Editor
{
    InsideSwitch insw;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        insw = target as InsideSwitch;

        if (insw.showInside == true)
        {
            if (GUILayout.Button("Cover inside")) ShowInside(false);
        }
        else
        {
            if (GUILayout.Button("Reveal inside")) ShowInside(true);
        }
    }

    private void ShowInside(bool show)
    {
        if (Application.isPlaying)
            insw.showInside = show;
        else
            insw.ShowInsideImmediate(show);
    }
}
