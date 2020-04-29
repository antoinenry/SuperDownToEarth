using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class ValueChangeEventExplorer
{
    public Predicate<ValueChangeEvent> filter;
    public GameObject selectedGameObject;

    public ValueChangeEvent SelectedVce { get => vceOptions == null ? null : vceOptions[selectedComponentIndex]; }
    public ValueChangeEventsComponent SelectedComponent { get => componentOptions == null ? null : componentOptions[selectedComponentIndex]; }
    
    private ValueChangeEventsComponent[] componentOptions;
    private List<string> componentOptionNames;
    private int selectedComponentIndex;

    private ValueChangeEvent[] vceOptions;
    private List<string> vceOptionNames;
    private int selectedVceIndex;

    public ValueChangeEventExplorer(Predicate<ValueChangeEvent> filter = null)
    {
        this.filter = filter;
    }

    public void EditorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        SelectGameObjectGUI();
        SelectComponentGUI();
        SelectEventGUI();
        EditorGUILayout.EndHorizontal();
    }

    private void SelectGameObjectGUI()
    {
        selectedGameObject = EditorGUILayout.ObjectField(selectedGameObject, typeof(GameObject), true) as GameObject;
    }

    private void SelectComponentGUI()
    {
        if (selectedGameObject == null)
        {
            componentOptions = null;
            componentOptionNames = new List<string> { "(no gameobject selected)" };
            selectedComponentIndex = 0;
        }
        else
        {
            componentOptions = selectedGameObject.GetComponents<ValueChangeEventsComponent>();

            if (componentOptions.Length == 0)
            {
                componentOptions = null;
                componentOptionNames = new List<string> { "(no event in gameobject" };
            }
            else
            {
                componentOptionNames = new List<ValueChangeEventsComponent>(componentOptions).ConvertAll(c => c.ToString());
            }
        }

        selectedComponentIndex = EditorGUILayout.Popup(selectedComponentIndex, componentOptionNames.ToArray());
    }

    private void SelectEventGUI()
    {
        if (SelectedComponent == null)
        {
            vceOptions = null;
            vceOptionNames = new List<string> { "(no component selected)" };
            selectedVceIndex = 0;
        }
        else
        {
            SelectedComponent.GetValueChangeEvents(out ValueChangeEvent[] vces);

            if(filter == null)
                vceOptions = vces;
            else
                vceOptions = new List<ValueChangeEvent>(vces).FindAll(filter).ToArray();

            if (vceOptions.Length == 0)
            {
                vceOptions = null;
                vceOptionNames = new List<string> { "(no ValueChangeEvent found)" };
            }
            else
            {
                vceOptionNames = new List<ValueChangeEvent>(vceOptions).ConvertAll(vce => vce.ToString());
            }
        }

        selectedVceIndex = EditorGUILayout.Popup(selectedVceIndex, vceOptionNames.ToArray());
    }
}
