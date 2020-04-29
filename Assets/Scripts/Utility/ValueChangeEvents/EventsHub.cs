using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsHub : MonoBehaviour
{
    [HideInInspector] [SerializeField] private Component[] eventHubComponents;

    private void Awake()
    {
        FetchEventHubElements();
    }

    public void FetchEventHubElements()
    {
        List<IEventsHubElement> eventHubElements = new List<IEventsHubElement>(GetComponentsInChildren<IEventsHubElement>(true));
        eventHubComponents = eventHubElements.ConvertAll<Component>(x => x as Component).ToArray();
    }

    public List<IEventsHubElement> GetEventHubElements()
    {
        if (eventHubComponents == null) return null;

        else return new List<Component>(eventHubComponents).ConvertAll<IEventsHubElement>(x => x as IEventsHubElement);

    }

    public void GetEventHubElements(out IEventsHubElement[] eventHubElements)
    {
        if (eventHubComponents == null) eventHubElements = null;

        else eventHubElements = new List<Component>(eventHubComponents).ConvertAll<IEventsHubElement>(x => x as IEventsHubElement).ToArray();
    }

    public IEventsHubElement GetEventHubElement(int index)
    {
        if (eventHubComponents == null || index < 0 || index > eventHubComponents.Length) return null;

        else return eventHubComponents[index] as IEventsHubElement;
    }
}
