using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EventsHubListener))]
public class EventsHubListenerInspector : Editor
{
    private EventsHubListener ehl;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ehl = target as EventsHubListener;


    }
}
