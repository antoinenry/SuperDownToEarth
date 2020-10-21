using System;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;

namespace Scarblab.VCE
{
    [Serializable]
    public struct ValueChangeEventID : IEquatable<ValueChangeEventID>
    {
        [SerializeField] private string name;
        [SerializeField] private Component component;
        [SerializeField] private int indexInComponent;

        public string Name { get => name; }
        public Component Component { get => component; }
        public int IndexInComponent { get => indexInComponent; }

        public ValueChangeEventID(int i, Component c)
        {
            if (c != null && i >= 0)
            {
                FieldInfo[] getFields = c.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (getFields != null)
                {
                    int vceCounter = 0;
                    foreach (FieldInfo field in getFields)
                    {
                        object fieldValue = field.GetValue(c);
                        if (fieldValue is ValueChangeEvent)
                        {
                            if (vceCounter == i)
                            {
                                name = field.Name;
                                component = c;
                                indexInComponent = vceCounter;
                                return;
                            }
                            else vceCounter++;
                        }
                    }
                }
            }

            indexInComponent = -1;
            component = null;
            name = "<noID";
        }

        public ValueChangeEventID(string n, Component c)
        {
            if (c != null && n != null)
            {
                FieldInfo[] getFields = c.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (getFields != null)
                {
                    int vceCounter = 0;
                    foreach (FieldInfo field in getFields)
                    {
                        object fieldValue = field.GetValue(c);
                        if (fieldValue is ValueChangeEvent)
                        {
                            if (field.Name == n)
                            {
                                name = n;
                                component = c;
                                indexInComponent = vceCounter;
                                return;
                            }
                            else vceCounter++;
                        }
                    }
                }
            }

            indexInComponent = -1;
            component = null;
            name = "<noID";
        }

        public static ValueChangeEventID None { get => new ValueChangeEventID(null, null); }

        public ValueChangeEvent ValueChangeEvent { get => FindValueChangeEvent(Component, Name); }

        public bool Equals(ValueChangeEventID other) => Name == other.Name && Component && other.Component;

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + (Name == null ? 0 : Name.GetHashCode());
                hash = (hash * 7) + (Component == null ? 0 : Component.GetHashCode());
                return hash;
            }
        }

        public static ValueChangeEvent FindValueChangeEvent(Component inComponent, string name)
        {
            if (inComponent == null) return null;

            FieldInfo getFieldByName = inComponent.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (getFieldByName != null)
            {
                object fieldValue = getFieldByName.GetValue(inComponent);
                if (fieldValue is ValueChangeEvent) return fieldValue as ValueChangeEvent;
            }
            return null;
        }

        public static ValueChangeEventID[] FindValueChangeEventsIDs(Component inComponent)
        {
            if (inComponent == null) return new ValueChangeEventID[0];

            List<ValueChangeEventID> ids = new List<ValueChangeEventID>();
            FieldInfo[] getFields = inComponent.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (getFields != null)
            {
                foreach (FieldInfo field in getFields)
                {
                    object fieldValue = field.GetValue(inComponent);
                    if (fieldValue is ValueChangeEvent)
                        ids.Add(new ValueChangeEventID(field.Name, inComponent));
                }
            }

            return ids.ToArray();
        }

        public static ValueChangeEvent[] FindValueChangeEvents(Component inComponent)
        {
            if (inComponent == null) return new ValueChangeEvent[0];

            List<ValueChangeEvent> vces = new List<ValueChangeEvent>();
            FieldInfo[] getFields = inComponent.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (getFields != null)
            {
                foreach (FieldInfo field in getFields)
                {
                    object fieldValue = field.GetValue(inComponent);
                    if (fieldValue is ValueChangeEvent)
                        vces.Add(fieldValue as ValueChangeEvent);
                }
            }

            return vces.ToArray();
        }

        public override string ToString()
        {
            if (component != null)
                return name + "[" + component.ToString() + "]";
            else
                return "NULL ValueChangeEvent (" + name + ")";
        }
    }
}