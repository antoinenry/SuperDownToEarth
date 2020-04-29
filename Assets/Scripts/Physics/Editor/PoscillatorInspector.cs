using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Poscillator))]
[CanEditMultipleObjects]
public class PoscillatorInspector : Editor
{
    private List<Poscillator> poscTargets;
    private float amp_min;
    private float amp_max;
    private float freq_min;
    private float freq_max;

    public override void OnInspectorGUI()
    {
        if(targets.Length == 1)
        {
            base.OnInspectorGUI();
        }
        else
        {
            poscTargets = new List<Object>(targets).ConvertAll<Poscillator>(t => t as Poscillator);
            MultiInspectorGUI();
        }
    }

    private void MultiInspectorGUI()
    {
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginHorizontal();
        amp_min = EditorGUILayout.FloatField("Amplitude", amp_min);
        amp_max = EditorGUILayout.FloatField(amp_max);
        amp_max = Mathf.Max(amp_min, amp_max);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        freq_min = EditorGUILayout.FloatField("Frequency", freq_min);
        freq_max = EditorGUILayout.FloatField(freq_max);
        freq_max = Mathf.Max(freq_min, freq_max);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        if (GUILayout.Button("Randomize oscillators"))
            RandomizeOscillators();
    }

    private void RandomizeOscillators()
    {
        foreach(Poscillator posc in poscTargets)
        {
            posc.amplitude = new Vector2(Random.Range(amp_min, amp_max), Random.Range(amp_min, amp_max));
            posc.frequency = new Vector2(Random.Range(freq_min, freq_max), Random.Range(freq_min, freq_max));
        }
    }
}
