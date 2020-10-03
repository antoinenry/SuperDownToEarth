﻿using System;
using UnityEngine;
using Scarblab.VCE;

[Serializable] public class Trigger : ValueChangeEvent { }

[Serializable]
public class BoolChangeEvent : ValueChangeEvent
{
    [SerializeField] private bool value;
    public static implicit operator bool(BoolChangeEvent vce) => (bool)vce.Value;
}

[Serializable]
public class IntChangeEvent : ValueChangeEvent
{
    [SerializeField] private int value;
    public static implicit operator int(IntChangeEvent vce) => (int)vce.Value;
}

[Serializable]
public class FloatChangeEvent : ValueChangeEvent
{
    [SerializeField] private float value;
    public static implicit operator float(FloatChangeEvent vce) => (float)vce.Value;
}

[Serializable]
public class StringChangeEvent : ValueChangeEvent
{
    [SerializeField] private string value;
    public static implicit operator string(StringChangeEvent vce) => (string)vce.Value;
}

[Serializable]
public class EnumChangeEvent : ValueChangeEvent
{
    [SerializeField] private int value;
    [SerializeField] private string[] enumNames;
    public static implicit operator int(EnumChangeEvent vce) => (int)vce.Value;
    public EnumChangeEvent(Type enumType)
    {
        if (enumType.IsEnum)
        {
            SetValueType(enumType);
            enumNames = enumType.GetEnumNames();
        }
        else
            Debug.LogError(enumType.ToString() + " is not an enum type");
    }

    protected override void SetValue(object valueObject, bool andTriggerEvent)
    {
        if ((int)valueObject != value)
        {
            value = (int)valueObject;
            if (andTriggerEvent) Trigger();
        }
    }
}

[Serializable]
public class Vector2ChangeEvent : ValueChangeEvent
{
    [SerializeField] private Vector2 value;
    public static implicit operator Vector2(Vector2ChangeEvent vce) => (Vector2)vce.Value;
}

[Serializable]
public class ColorChangeEvent : ValueChangeEvent
{
    [SerializeField] private Color value;
    public static implicit operator Color(ColorChangeEvent vce) => (Color)vce.Value;
}

[Serializable]
public class ComponentChangeEvent : ValueChangeEvent
{
    [SerializeField] private Component value;
    public static implicit operator Component(ComponentChangeEvent vce) => (Component)vce.Value;
}