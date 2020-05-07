using System;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;

public abstract class ValueChangeEventsBehaviour : MonoBehaviour, IValueChangeEventsComponent
{
    private ValueChangeEvent[] valueChangeEvents;

    public virtual int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        if (valueChangeEvents == null)
        {
            Type componentType = GetType();
            FieldInfo[] componentFields = componentType.GetFields();

            List<ValueChangeEvent> vceList = new List<ValueChangeEvent>();
            if (componentFields != null)
                foreach (FieldInfo field in componentFields)
                    if (field.FieldType == typeof(ValueChangeEvent))
                    {
                        ValueChangeEvent vce = field.GetValue(this) as ValueChangeEvent;
                        vceList.Add(vce);
                    }

            valueChangeEvents = vceList.ToArray();
        }

        vces = new ValueChangeEvent[valueChangeEvents.Length];
        valueChangeEvents.CopyTo(vces, 0);
        return valueChangeEvents.Length;
    }

    public virtual ValueChangeEvent GetValueChangeEventByName(string vceName)
    {
        Type componentType = GetType();
        FieldInfo vceField = componentType.GetField(vceName);

        if (vceField != null)
            return vceField.GetValue(this) as ValueChangeEvent;
        else
            return null;
    }

    public virtual string[] GetValueChangeEventsNames()
    {
        Type componentType = GetType();
        FieldInfo[] componentFields = componentType.GetFields();
        List<string> nameList = new List<string>();

        if (componentFields != null)
            foreach (FieldInfo field in componentFields)
                if (field.FieldType == typeof(ValueChangeEvent))
                    nameList.Add(field.Name);

        return nameList.ToArray();
    }

    public virtual void ResetRuntimeValueChangeEvents()
    {
        GetValueChangeEvents(out ValueChangeEvent[] vces);
        foreach (ValueChangeEvent vce in vces)
            vce.ResetRuntimeEvent();
    }

    public virtual void EnslaveValueChangeEvents(bool enslave)
    {
        GetValueChangeEvents(out ValueChangeEvent[] vces);
        foreach (ValueChangeEvent vce in vces)
            vce.Enslave(enslave);
    }

    public virtual void Awake()
    {
        ResetRuntimeValueChangeEvents();
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
