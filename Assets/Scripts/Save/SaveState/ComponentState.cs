using System;
using UnityEngine;

[Serializable]
public abstract class ComponentState
{
    public abstract Component Component { get; }
    public abstract Type Type { get;  }

    public static ComponentState New(Component fromComponent)
    {

        return null;
    }
}