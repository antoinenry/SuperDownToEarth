﻿using System;
using UnityEngine;

[Serializable]
public struct ValueChangeEventID
{
    public string name;
    public Component component;
    public int indexInComponent;

    public ValueChangeEvent ValueChangeEvent
    {
        get
        {
            if (component != null && component is ValueChangeEventsComponent)
            {
                (component as ValueChangeEventsComponent).GetValueChangeEvents(out ValueChangeEvent[] vces);
                if (indexInComponent >= 0 && indexInComponent < vces.Length) return vces[indexInComponent];
            }

            return null;
        }
    }
}
