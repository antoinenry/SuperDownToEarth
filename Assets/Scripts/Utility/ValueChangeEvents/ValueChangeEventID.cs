using System;
using UnityEngine;

[Serializable]
public struct ValueChangeEventID
{
    public string name;
    public Component component;
    public int indexInComponent;

    public ValueChangeEvent ValueChangeEvent
    {
        get => GetValueChangeEvent(component, indexInComponent);
    }

    public static ValueChangeEvent GetValueChangeEvent(Component component, int indexInComponent)
    {
        if (component != null && component is IValueChangeEventsComponent)
        {
            (component as IValueChangeEventsComponent).GetValueChangeEvents(out ValueChangeEvent[] vces);
            if (indexInComponent >= 0 && indexInComponent < vces.Length) return vces[indexInComponent];
        }

        return null;
    }

    public static void SetAll(GameObject gameObject)
    {
        if (gameObject == null) return;

        IValueChangeEventsComponent[] components = gameObject.GetComponents<IValueChangeEventsComponent>();
        if (components != null)
            foreach (IValueChangeEventsComponent component in components)
                component.SetValueChangeEventsID();
    }

    public override string ToString()
    {
        return "ID (" + component + "/" + name + "(" + indexInComponent + ")";
    }
}
