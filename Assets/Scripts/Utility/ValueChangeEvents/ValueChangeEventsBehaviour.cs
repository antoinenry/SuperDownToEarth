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
            //Debug.Log("AutoSet VCE events in " + ToString());
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
                        //Debug.Log("-" + vce.ToString());
                        vceList.Add(vce);
                    }
                }
            }

            valueChangeEvents = vceList.ToArray();
        }

        vces = new ValueChangeEvent[valueChangeEvents.Length];
        valueChangeEvents.CopyTo(vces, 0);
        return valueChangeEvents.Length;
    }

    public ValueChangeEvent GetValueChangeEventByName(string vceName)
    {
        Type componentType = GetType();
        FieldInfo vceField = componentType.GetField(vceName);

        if (vceField != null)
            return vceField.GetValue(this) as ValueChangeEvent;
        else
            return null;
    }

    public string[] GetValueChangeEventsNames()
    {
        Type componentType = GetType();
        FieldInfo[] componentFields = componentType.GetFields();
        List<string> nameList = new List<string>();

        if (componentFields != null)
        {
            foreach (FieldInfo field in componentFields)
            {
                if (field.FieldType == typeof(ValueChangeEvent))
                {
                    nameList.Add(field.Name);
                }
            }
        }

        return nameList.ToArray();
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
