using System;
using UnityEngine;
using Scarblab.VCE;

[Serializable] public class Trigger : ValueChangeEvent { }

[Serializable] public class BoolChangeEvent : ValueChangeEvent
{
    [SerializeField] private bool value;
    public static implicit operator bool(BoolChangeEvent vce) => (bool)vce.Value;
}

[Serializable] public class IntChangeEvent : ValueChangeEvent
{
    [SerializeField] private int value;
    public static implicit operator int(IntChangeEvent vce) => (int)vce.Value;
}

[Serializable] public class FloatChangeEvent : ValueChangeEvent
{
    [SerializeField] private float value;
    public static implicit operator float(FloatChangeEvent vce) => (float)vce.Value;
}

[Serializable] public class ComponentChangeEvent : ValueChangeEvent
{
    [SerializeField] private Component value;
    public static implicit operator Component(ComponentChangeEvent vce) => (Component)vce.Value;
}

[Serializable] public class EnumChangeEvent : ValueChangeEvent
{
    [SerializeField] private int value;
    [SerializeField] private string[] enumNames;
    public static implicit operator int(EnumChangeEvent vce) => (int)vce.Value;
    public EnumChangeEvent(Type enumType)
    {
        if (enumType.IsEnum)
            enumNames = enumType.GetEnumNames();
        else
            Debug.LogError(enumType.ToString() + " is not an enum type");
    }
}