using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EventsHub))]
public class EventsHubInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EventsHub eh = target as EventsHub;
        List<IEventsHubElement> eventHubElements = eh.GetEventHubElements();

        if (eventHubElements != null)
            foreach (IEventsHubElement hubElement in eventHubElements)
            {
                

                if (hubElement != null)
                {
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField(hubElement.ToString(), EditorStyles.boldLabel);
                    IValueChangeEventsGUI(hubElement);
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    EditorGUILayout.HelpBox("Missing hub element", MessageType.Warning);
                }                
            }

        if (GUILayout.Button("Fetch hub events")) eh.FetchEventHubElements();
    }

    private void IValueChangeEventsGUI(IEventsHubElement hubElement)
    {
        hubElement.GetValueChangeEventsNamesAndTypes(out string[] names, out Type[] types);
        if (names != null && types != null)
        {
            for (int i = 0, imax = names.Length; i < imax; i++)
                if (hubElement.GetValueChangeEvent(i, out IValueChangeEvent vce))
                    ValueChangeEventGUI(names[i], vce);
        }
    }

    private void ValueChangeEventGUI(string name, IValueChangeEvent vce)
    {
        Type valueType = vce.GetValueType();
        string label = name + " (" + ((valueType == null) ? "Trigger" : valueType.Name) + ")";

        if (valueType == null)
            TriggerEventGUI(label, vce as TriggerEvent);
        else if (valueType == typeof(bool))
            BoolValueChangeEventGUI(label, vce as ValueChangeEvent<bool>);
        else if (valueType == typeof(int))
            IntValueChangeEventGUI(label, vce as ValueChangeEvent<int>);
        else if (valueType == typeof(float))
            FloatValueChangeEventGUI(label, vce as ValueChangeEvent<float>);
    }

    private void TriggerEventGUI(string label, TriggerEvent triggerEvent)
    {
        if (triggerEvent == null)
            EditorGUILayout.Toggle(label, false);
        else
        {
            bool triggered = EditorGUILayout.Toggle(label, triggerEvent.triggered);
            if (triggered == true && triggerEvent.triggered == false)
                triggerEvent.Trigger();
        }
    }

    private void BoolValueChangeEventGUI(string label, ValueChangeEvent<bool> boolChangeEvent)
    {
        if (boolChangeEvent == null)
            EditorGUILayout.Toggle(label, false);
        else
            boolChangeEvent.Value = EditorGUILayout.Toggle(label, boolChangeEvent.Value);
    }

    private void IntValueChangeEventGUI(string label, ValueChangeEvent<int> intChangeEvent)
    {
        if (intChangeEvent == null)
            EditorGUILayout.IntField(label, 0);
        else
            intChangeEvent.Value = EditorGUILayout.IntField(label, intChangeEvent.Value);
    }

    private void FloatValueChangeEventGUI(string label, ValueChangeEvent<float> floatChangeEvent)
    {
        if (floatChangeEvent == null)
            EditorGUILayout.IntField(label, 0);
        else
            floatChangeEvent.Value = EditorGUILayout.FloatField(label, floatChangeEvent.Value);
    }
}
