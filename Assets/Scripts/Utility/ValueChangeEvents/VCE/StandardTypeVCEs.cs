using System;
using UnityEngine;
using VCE;

[Serializable] public class Trigger : ValueChangeEvent { }
[Serializable] public class BoolChangeEvent : ValueChangeEvent { [SerializeField] private bool value; }
[Serializable] public class IntChangeEvent : ValueChangeEvent {[SerializeField] private int value; }
[Serializable] public class FloatChangeEvent : ValueChangeEvent {[SerializeField] private float value; }
[Serializable] public class ObjectChangeEvent : ValueChangeEvent {[SerializeField] private UnityEngine.Object value; }
