using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class ValueChangeEventExplorer
{
    public Predicate<ValueChangeEventID> filter;
    public GameObject selectedGameObject;

    public ValueChangeEventID SelectedVceID { get => vceIDOptions == null ? new ValueChangeEventID() : vceIDOptions[selectedVceIndex]; }
    public Component SelectedComponent { get => componentOptions == null ? null : componentOptions[selectedComponentIndex]; }
    
    private List<Component> componentOptions;
    private List<string> componentOptionNames;
    private int selectedComponentIndex;

    private ValueChangeEventID[] vceIDOptions;
    private List<string> vceOptionNames;
    private int selectedVceIndex;

    public ValueChangeEventExplorer(Predicate<ValueChangeEventID> filter = null)
    {
        Debug.Log("New ValueChangeExplorer");
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
            componentOptions = new List<Component>(selectedGameObject.GetComponents<Component>()).FindAll(c => c is IValueChangeEventsComponent);

            if (componentOptions.Count == 0)
                componentOptionNames = new List<string> { "(no event in gameobject" };
            else
                componentOptionNames = new List<Component>(componentOptions).ConvertAll(c => c.ToString());
        }

        selectedComponentIndex = EditorGUILayout.Popup(selectedComponentIndex, componentOptionNames.ToArray());
    }

    private void SelectEventGUI()
    {
        if (SelectedComponent == null)
        {
            vceIDOptions = null;
            vceOptionNames = new List<string> { "(no component selected)" };
            selectedVceIndex = 0;
        }
        else
        {
            IValueChangeEventsComponent selectedComponent = SelectedComponent as IValueChangeEventsComponent;
            selectedComponent.GetValueChangeEvents(out ValueChangeEvent[] vces);
            List<string> vceNames = new List<string>(selectedComponent.GetValueChangeEventsNames());
            List<ValueChangeEventID> vceIDs = vceNames.ConvertAll(name => new ValueChangeEventID(SelectedComponent, name));

            if(filter == null)
                vceIDOptions = vceIDs.ToArray();
            else
                vceIDOptions = vceIDs.FindAll(filter).ToArray();

            if (vceIDOptions.Length == 0)
            {
                vceIDOptions = null;
                vceOptionNames = new List<string> { "(no type match)" };
            }
            else
            {
                vceOptionNames = new List<ValueChangeEventID>(vceIDOptions).ConvertAll(id => id.name);
            }
        }

        selectedVceIndex = EditorGUILayout.Popup(selectedVceIndex, vceOptionNames.ToArray());
    }
}
