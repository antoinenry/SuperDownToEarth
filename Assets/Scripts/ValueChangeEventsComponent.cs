using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ValueChangeEventsComponent : MonoBehaviour
{
    public abstract void SetValueChangeEventsID();
    public abstract int GetValueChangeEvents(out ValueChangeEvent[] vces);
    public abstract int GetNewRuntimeEvents(out IValueChangeEvent[] newRuntimeEvents);

    public virtual void Awake()
    {
        SetValueChangeEventsID();
        int eventCount = GetValueChangeEvents(out ValueChangeEvent[] vces);
        int runtimeCount = GetNewRuntimeEvents(out IValueChangeEvent[] newRuntimeEvents);

        if (runtimeCount == eventCount)
        {
            for (int i = 0; i < eventCount; i++)
                if (vces[i].runtimeEvent == null) vces[i].runtimeEvent = newRuntimeEvents[i];
        }
        else Debug.LogError("ValueChangeEvents (" + eventCount + ") and RuntimeEvents (" + runtimeCount + ") count mismatch.");
    }

    public virtual void Start()
    {
        GetValueChangeEvents(out ValueChangeEvent[] vces);

        foreach (ValueChangeEvent vce in vces)
            vce.Enslave();
    }

}