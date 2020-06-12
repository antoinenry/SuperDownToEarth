using UnityEditor;

[CustomEditor(typeof(Circuit))]
public class CircuitEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        Circuit circuit = target as Circuit;
        circuit.UpdateTrajectory();
        EditorUtility.SetDirty(circuit);
    }
}
