using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public struct ValueChangeEventID
{
    public string name;
    public string componentTypeName;
    public int componentIndex;
    public int indexInComponent;

    public Type ComponentType
    {
        get => Type.GetType(componentTypeName);
        set => componentTypeName = value.AssemblyQualifiedName;
    }

    public bool GetValueChangeEvent(GameObject inGameObject, out ValueChangeEvent vce)
    {
        vce = null;
        if (inGameObject == null)
        {
            Debug.LogError("Missing object reference");
            return false;
        }

        Component inComponent = GetComponent(inGameObject);

        if (inComponent != null && inComponent is IValueChangeEventsComponent)
        {
            (inComponent as IValueChangeEventsComponent).GetValueChangeEvents(out ValueChangeEvent[] vces);
            if (indexInComponent >= 0 && indexInComponent < vces.Length)
            {
                vce = vces[indexInComponent];
                return true;
            }
        }

        vce = null;
        return false;
    }

    public Component GetComponent(GameObject inGameObject)
    {
        if (indexInComponent < 0) return null;

        IValueChangeEventsComponent[] allComponents = inGameObject.GetComponents<IValueChangeEventsComponent>();
        List<IValueChangeEventsComponent> matchingTypeComponents = new List<IValueChangeEventsComponent>();
        foreach (IValueChangeEventsComponent component in allComponents)
        {
            if (component.GetType() == ComponentType) matchingTypeComponents.Add(component);
        }

        if (componentIndex < matchingTypeComponents.Count)
            return matchingTypeComponents[componentIndex] as Component;
        else
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
        return "ID (" + componentTypeName + "[" + componentIndex + "]." + name + "(" + indexInComponent + ")";
    }
}