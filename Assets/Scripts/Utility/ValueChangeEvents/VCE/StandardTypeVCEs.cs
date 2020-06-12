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

[Serializable] public class UnityObjectChangeEvent : ValueChangeEvent
{
    [SerializeField] private UnityEngine.Object value;
    public static implicit operator UnityEngine.Object(UnityObjectChangeEvent vce) => (UnityEngine.Object)vce.Value;
}