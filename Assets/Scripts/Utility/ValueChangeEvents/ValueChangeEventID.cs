using System;

using UnityEngine;

[Serializable]
public struct ValueChangeEventID
{
    public Component component;
    public string name;

    public static ValueChangeEventID NoID { get => new ValueChangeEventID(null, "no_ID"); }

    public ValueChangeEventID(Component component, string name) { this.component = component; this.name = name; }

    public static ValueChangeEvent GetValueChangeEvent(Component inComponent, string vceName)
    {
        if (inComponent != null && inComponent is IValueChangeEventsComponent)
            return (inComponent as IValueChangeEventsComponent).GetValueChangeEventByName(vceName);
        else
            return null;
    }

    public ValueChangeEvent GetValueChangeEvent()
    {
        return GetValueChangeEvent(component, name);
    }

    public Type GetValueType()
    {
        ValueChangeEvent vce = GetValueChangeEvent();
        if (vce == null) return null;
        else return vce.ValueType;
    }

    public override string ToString()
    {
        Type valueType = GetValueType();
        if (valueType == null)
            return "vceID:" + component + "." + name + "(null type)";
        else
            return "vceID:" + component + "." + name + "(" + valueType.Name + ")";
    }
}