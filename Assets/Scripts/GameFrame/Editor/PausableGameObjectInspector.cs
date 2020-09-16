using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PausableGameObject))]
public class PausableGameObjectInspector : Editor
{
    private PausableGameObject pausableGO;
    private Component[] availablePausableComponents;
    //private List<Component> activePausableComponents;
    private bool unfoldActiveComponents;
    private bool unfoldInactiveComponents;

    public override void OnInspectorGUI()
    {
        if (pausableGO == null)
        {
            pausableGO = target as PausableGameObject;
            pausableGO.GetAllPausableComponents(out availablePausableComponents);
        }

        RootSelectionGUI();
        ComponentSelectionGUI();
    }

    private void RootSelectionGUI()
    {
        GameObject rootSelection = EditorGUILayout.ObjectField("Root", pausableGO.root, typeof(GameObject), true) as GameObject;
        if (rootSelection != pausableGO.root)
        {
            pausableGO.root = rootSelection;
            pausableGO.GetAllPausableComponents(out availablePausableComponents);
            pausableGO.pausableComponents = availablePausableComponents;
        }
    }

    private void ComponentSelectionGUI()
    {
        List<Component> activeComponents = new List<Component>(pausableGO.pausableComponents);
        bool applyChanges = false;

        int activeCount = activeComponents.Count;
        if (activeCount > 0)
        {
            unfoldActiveComponents = EditorGUILayout.Foldout(unfoldActiveComponents, "Components to pause (" + activeCount + ")");
            if (unfoldActiveComponents)
            {
                EditorGUILayout.BeginVertical("box");
                foreach (Component c in pausableGO.pausableComponents)
                {
                    if (EditorGUILayout.Toggle(c.ToString(), true) == false)
                    {
                        activeComponents.Remove(c);
                        unfoldInactiveComponents = true;
                        applyChanges = true;
                        break;
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }
        else
            unfoldActiveComponents = false;

        int inactiveCount = availablePausableComponents.Length - activeCount;
        if (inactiveCount > 0)
        {
            unfoldInactiveComponents = EditorGUILayout.Foldout(unfoldInactiveComponents, "Ignored components(" + inactiveCount + ")");
            if (unfoldInactiveComponents)
            {
                EditorGUILayout.BeginVertical("box");
                foreach (Component c in availablePausableComponents)
                {
                    if (activeComponents.Contains(c))
                        continue;

                    if (EditorGUILayout.Toggle(c.ToString(), false) == true)
                    {
                        activeComponents.Add(c);
                        unfoldActiveComponents = true;
                        applyChanges = true;
                        break;
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }
        else
            unfoldInactiveComponents = false;

        if (applyChanges) pausableGO.pausableComponents = activeComponents.ToArray();
    }
}
