using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsHubListener : MonoBehaviour
{
    /*
    [System.Serializable]
    public struct ValueChangeEventsLink
    {
        public IValueChangeEvent master;
        public IValueChangeEvent slave;

        public bool enabled { get; private set; }

        public void AddListeners()
        {
            if (enabled == false && master != null && slave != null)
            {
                enabled = true;
                slave.EnslaveTo(master);
            }
        }

        public void RemoveListeners()
        {
            if (enabled == true && master != null && slave != null)
            {
                enabled = false;
                slave.FreeFrom(master);
            }
        }
    }

    [HideInInspector] public ValueChangeEventsLink[] eventLinks;

    private void OnEnable()
    {
        if(eventLinks != null)
        {
            foreach (ValueChangeEventsLink link in eventLinks)
                link.AddListeners();
        }
    }

    private void OnDisable()
    {
        if (eventLinks != null)
        {
            foreach (ValueChangeEventsLink link in eventLinks)
                link.RemoveListeners();
        }
    }
    */
}
