using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class ValueChangeEventEditorWindow : EditorWindow
{   
    private ValueChangeEventComponentEditor[] inspectedComponents;

    [MenuItem("Window/ValueChangeEvents")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ValueChangeEventEditorWindow));
    }

    private void OnHierarchyChange()
    {
        FetchComponents();
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnSelectionChange()
    {
        FetchComponents();
    }    

    private void OnGUI()
    {
        if (inspectedComponents == null)
            FetchComponents();

        if (inspectedComponents.Length == 0)
            EditorGUILayout.HelpBox("No ValueChangeEvents in selection", MessageType.None);
        else
        {
            foreach (ValueChangeEventComponentEditor componentEditor in inspectedComponents)
            {
                componentEditor.EditorGUI();
            }
        }
    }

    private void FetchComponents()
    {
        if (Selection.gameObjects == null)
            inspectedComponents = new ValueChangeEventComponentEditor[0];
        else
        {
            List<ValueChangeEventComponentEditor> fetchList = new List<ValueChangeEventComponentEditor>();
            foreach (GameObject selected in Selection.gameObjects)
            {
                IValueChangeEventsComponent[] fetchedComponents = selected.GetComponents<IValueChangeEventsComponent>();
                foreach(IValueChangeEventsComponent iComponent in fetchedComponents)
                    fetchList.Add(new ValueChangeEventComponentEditor(iComponent as Component));
            }
            
            inspectedComponents = fetchList.ToArray();
        }
    }     
}
