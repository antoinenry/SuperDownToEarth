using System;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;

public abstract class ValueChangeEventsBehaviour : MonoBehaviour, IValueChangeEventsComponent
{
    private ValueChangeEvent[] valueChangeEvents;

    public int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        if (valueChangeEvents == null)
        {
            Debug.Log("AutoSet VCE events in " + ToString());
            Type componentType = GetType();
            FieldInfo[] componentFields = componentType.GetFields();

            List<ValueChangeEvent> vceList = new List<ValueChangeEvent>();
            if (componentFields != null)
            {
                foreach (FieldInfo field in componentFields)
                {
                    if (field.FieldType == typeof(ValueChangeEvent))
                    {
                        ValueChangeEvent vce = field.GetValue(this) as ValueChangeEvent;
                        Debug.Log("-" + vce.ToString());
                        vceList.Add(vce);
                    }
                }
            }

            valueChangeEvents = vceList.ToArray();
        }

        Debug.Log("VCE events already set in " + ToString() + " - " + valueChangeEvents.Length + " events.");

        vces = new ValueChangeEvent[valueChangeEvents.Length];
        valueChangeEvents.CopyTo(vces, 0);
        return valueChangeEvents.Length;
    }

    public ValueChangeEvent GetValueChangeEventByName(string vceName)
    {
        SetValueChangeEventsID();
        GetValueChangeEvents(out ValueChangeEvent[] vces);
        foreach (ValueChangeEvent vce in vces)
            if (vce.Name == vceName) return vce;

        return null;
    }

    public int SetValueChangeEventsID()
    {
        int vceCount = GetValueChangeEvents(out ValueChangeEvent[] vces);
        if (vceCount == 0) return 0;

        Type componentType = GetType();
        FieldInfo[] componentFields = componentType.GetFields();

        int vceIndex = 0;
        if(componentFields != null)
        {
            foreach (FieldInfo field in componentFields)
            {
                if (field.FieldType == typeof(ValueChangeEvent))
                    vces[vceIndex].SetID(field.Name, this, vceIndex++);
            }
        }

        return vceIndex;
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
