using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class ValueChangeEventEditorWindow : EditorWindow
{   
    private ValueChangeEventComponentEditor[] inspectedComponents;
    private GameObject vceExplorerLastSelectedGameObject;

    [MenuItem("Window/ValueChangeEvents")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ValueChangeEventEditorWindow));
    }

    private void OnHierarchyChange()
    {
        UpdateComponents();
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
        UpdateComponents();

        if (inspectedComponents.Length == 0)
        {
            EditorGUILayout.HelpBox("No ValueChangeEvents in selection. ", MessageType.None);
        }
        else
        {
            foreach (ValueChangeEventComponentEditor componentEditor in inspectedComponents)
            {
                componentEditor.EditorGUI();
            }
        }
    }

    private void UpdateComponents()
    {
        bool reset = (inspectedComponents == null);

        if (reset == false)
        {
            foreach(ValueChangeEventComponentEditor componentEditor in inspectedComponents)
            {
                if (componentEditor.component == null)
                {
                    reset = true;
                    break;
                }
            }
        }

        if (reset) FetchComponents();
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
                foreach (IValueChangeEventsComponent iComponent in fetchedComponents)
                {
                    ValueChangeEventComponentEditor componentEditor = new ValueChangeEventComponentEditor(iComponent as Component, vceExplorerLastSelectedGameObject);
                    fetchList.Add(componentEditor);
                    vceExplorerLastSelectedGameObject = componentEditor.lastSelectedGameObject;
                }
            }
            
            inspectedComponents = fetchList.ToArray();
        }
    }  
}
