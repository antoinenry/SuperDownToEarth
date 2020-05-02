using System;
using UnityEngine;

[Serializable]
public struct ValueChangeEventID
{
    public string name;
    public Component component;
    public int indexInComponent;

    public static bool GetValueChangeEvent(ValueChangeEventID id, out ValueChangeEvent vce)
    {
        return GetValueChangeEvent(id.component, id.indexInComponent, out vce);
    }

    public static bool GetValueChangeEvent(Component component, int indexInComponent, out ValueChangeEvent vce)
    {
        if (component != null && component is IValueChangeEventsComponent)
        {
            (component as IValueChangeEventsComponent).GetValueChangeEvents(out ValueChangeEvent[] vces);
            if (indexInComponent >= 0 && indexInComponent < vces.Length)
            {
                vce = vces[indexInComponent];
                return true;
            }
        }

        vce = null;
        return false;
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
