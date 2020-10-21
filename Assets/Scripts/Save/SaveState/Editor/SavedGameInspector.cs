using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SavedGame))]
public class SavedGameInspector : Editor
{
    public override void OnInspectorGUI()
    {
        SavedGame savedGame = target as SavedGame;
        int stateCount = savedGame.StateCount;

        EditorGUILayout.LabelField(stateCount + " saved states");

        for (int i = 0; i < stateCount; i++)
        {
            ComponentState state = savedGame.savedStates[i];
            if (state == null)
                EditorGUILayout.HelpBox("Missing state reference", MessageType.Error);
            else if (state.Component == null)
                EditorGUILayout.HelpBox("Missing component reference", MessageType.Error);
            else
            {
                EditorGUILayout.ObjectField("State " + i, state.Component, state.Type, true);
                ComponentStateGUILayout(state);
            }
        }
    }

    private void ComponentStateGUILayout(ComponentState savedState)
    {
        if (savedState is TransformState) TransformStateGUILayout(savedState as TransformState);
    }

    private void TransformStateGUILayout(TransformState savedState)
    {

    }
}
