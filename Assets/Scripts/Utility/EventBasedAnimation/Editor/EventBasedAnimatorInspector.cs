using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EventBasedAnimator))]
public class EventBasedAnimatorInspector : Editor
{
    private EventBasedAnimator eba;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        eba = target as EventBasedAnimator;
        EventsHub eventsHub = eba.GetAnimatedObject();
        if (eventsHub != null)
        {
            eventsHub.FetchEventHubElements();

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Animation configurations", EditorStyles.boldLabel);

            int i = 0;
            bool remove = false;
            for(int imax = eba.NumAnimationConfigurations; i < imax; i++)
            {
                eba.SetAnimationConfiguration(i, AnimationConfigurationGUI(eventsHub, eba.GetAnimationConfiguration(i), out remove));
                if (remove) break;
            }

            if (remove)
            {
                eba.RemoveAnimationConfiguration(i);                
            }

            if(GUILayout.Button("Add new animation"))
            {
                eba.AddAnimationconfiguration();
            }

            EditorGUILayout.EndVertical();
        }
    }

    private EventBasedAnimator.EventAnimationConfiguration AnimationConfigurationGUI(EventsHub eventsHub, EventBasedAnimator.EventAnimationConfiguration conf, out bool remove)
    {
        EventBasedAnimator.EventAnimationConfiguration newConf = new EventBasedAnimator.EventAnimationConfiguration();

        EditorGUILayout.BeginHorizontal();

        List<IEventsHubElement> hubElements = eventsHub.GetEventHubElements();
        IEventsHubElement currentHubElement = (conf.eventHubElement as IEventsHubElement);

        List<string> hubElementsNames = hubElements.ConvertAll<string>(e => e.ToString());
        string[] eventNames = new string[0];
        Type[] eventsTypes = new Type[0];
        int currentHubElementIndex = 0;
        int currentEventIndex = 0;

        if (currentHubElement != null)
        {
            currentHubElementIndex = hubElements.IndexOf(currentHubElement);
            currentHubElement.GetValueChangeEventsNamesAndTypes(out eventNames, out eventsTypes);
            currentEventIndex = currentHubElement.GetValueChangeEventIndex(conf.eventParameterName);
        }

        if (currentHubElementIndex < 0 || currentHubElementIndex > hubElementsNames.Count - 1) currentHubElementIndex = 0;
        currentHubElementIndex = EditorGUILayout.Popup(currentHubElementIndex, hubElementsNames.ToArray());
        if (hubElements.Count > 0) newConf.eventHubElement = hubElements[currentHubElementIndex] as Component;

        List<string> eventNamesAndTypes = new List<string>();
        for (int i = 0, imax = eventNames.Length; i < imax; i++)
            eventNamesAndTypes.Add(eventNames[i] + GetTypeLabel(eventsTypes[i]));

        if (currentEventIndex < 0 || currentEventIndex > eventNames.Length - 1) currentEventIndex = 0;
        currentEventIndex = EditorGUILayout.Popup(currentEventIndex, eventNamesAndTypes.ToArray());
        if (eventNames.Length > 0)
        {
            if (conf.eventParameterName != eventNames[currentEventIndex])
                newConf.animatorParameterName = eventNames[currentEventIndex];
            else
                newConf.animatorParameterName = conf.animatorParameterName;

            newConf.eventParameterName = eventNames[currentEventIndex];
        }
        
        EditorGUILayout.LabelField("->", GUILayout.Width(30f));
        
        newConf.animatorParameterName = EditorGUILayout.TextField(newConf.animatorParameterName);

        remove = GUILayout.Button("-", GUILayout.Width(15f), GUILayout.Height(15f));

        EditorGUILayout.EndHorizontal();

        return newConf;
    }

    private string GetTypeLabel(Type type)
    {
        if (type == null) return " (trigger)";
        if (type == typeof(bool)) return " (bool)";
        if (type == typeof(int)) return " (int)";
        if (type == typeof(float)) return " (float)";

        return "";
    }
}
