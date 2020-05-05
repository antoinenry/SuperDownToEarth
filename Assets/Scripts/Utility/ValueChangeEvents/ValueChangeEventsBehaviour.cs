using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ValueChangeEventsBehaviour : MonoBehaviour, IValueChangeEventsComponent
{
    public virtual int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        vces = new ValueChangeEvent[0];
        return 0;
    }

    public virtual int SetValueChangeEventsID()
    {
        return 0;
    }

    public void EnslaveValueChangeEvents(bool enslave)
    {
        GetValueChangeEvents(out ValueChangeEvent[] vces);
        foreach (ValueChangeEvent vce in vces)
            vce.Enslave(enslave);
    }

    public virtual void OnEnable()
    {
        EnslaveValueChangeEvents(true);
    }

    public virtual void OnDisable()
    {
        EnslaveValueChangeEvents(false);
    }
}
