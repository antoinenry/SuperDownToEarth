using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class ValueChangeEvent<T> : UnityEvent<T>, IValueChangeEvent
{
    private T _value;
    private ValueChangeEvent<T>[] masters;

    //public string Name { get; private set; }
    public UnityAction<T> SetValueAction { get; private set; }

    public T Value
    {
        get { return _value; }
        set
        {
            if (_value == null)
            {
                if (value != null)
                {
                    _value = value;
                    Invoke(_value);
                }

                return;
            }

            if (_value.Equals(value) == false)
            {
                _value = value;
                Invoke(_value);
            }
        }
    }

    public ValueChangeEvent(T initValue = default(T), string name = "NewValueChangeEvent")
    {
        //this.Name = name;
        SetValueAction = new UnityAction<T>(x => Value = x);
        _value = initValue;
        masters = null;
    }

    public void ForceInvoke()
    {
        Invoke(_value);
    }

    public Type GetValueType()
    {
        return typeof(T);
    }

    public int GetMasterCount()
    {
        RemoveFromMastersArray(null);
        return masters == null ? 0 : masters.Length;
    }

    public void EnslaveTo(ValueChangeEvent other)
    {
        if (other == null)
        {
            Debug.LogError("Trying to enslave event to null.");
            return;
        }
        
        if (other.runtimeEvent == null)
            other.runtimeEvent = new ValueChangeEvent<T>();
        
        if (other.runtimeEvent is ValueChangeEvent<T>)
        {
            ValueChangeEvent<T> newMaster = other.runtimeEvent as ValueChangeEvent<T>;
            int currentMasterCount = GetMasterCount();

            if (currentMasterCount == 0)
                masters = new ValueChangeEvent<T>[] { newMaster };
            else
            {
                foreach (ValueChangeEvent<T> oldMasters in masters)
                    if (newMaster == oldMasters) return;
                
                Array.Resize(ref masters, currentMasterCount + 1);
                masters[currentMasterCount] = newMaster;
            }

            newMaster.AddListener(SetValueAction);
        }
        else
            Debug.LogError("Trying to enslave type " + GetValueType().Name + " to type " + other.ValueType.Name);

    }

    public void FreeFrom(ValueChangeEvent other)
    {
        if (other == null || other.runtimeEvent == null)
        {
            Debug.LogError("Trying to free event from null.");
            return;
        }

        if (other.runtimeEvent is ValueChangeEvent<T>)
        {
            ValueChangeEvent<T> oldMaster = other.runtimeEvent as ValueChangeEvent<T>;
            oldMaster.RemoveListener(SetValueAction);
            RemoveFromMastersArray(oldMaster);
        }
        else
            Debug.LogError("Trying to free type " + GetValueType().Name + " from type " + other.ValueType.Name);
    }

    private void RemoveFromMastersArray(ValueChangeEvent<T> remove)
    {
        if (masters != null)
        {
            List<ValueChangeEvent<T>> clean = new List<ValueChangeEvent<T>>(masters);
            if (clean.RemoveAll(m => m == remove) > 0) masters = clean.ToArray();

            if (masters.Length == 0) masters = null;
        }
    }
}