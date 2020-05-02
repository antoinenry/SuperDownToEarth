using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class ValueChangeEvent<T> : UnityEvent<T>, IValueChangeEvent
{
    private T _value;
    private ValueChangeEvent<T>[] masters;
    private bool invoked;

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
                    ForceInvoke();
                }

                return;
            }

            if (_value.Equals(value) == false)
            {
                _value = value;
                ForceInvoke();
            }
        }
    }

    public ValueChangeEvent(T initValue = default(T), string name = "NewValueChangeEvent")
    {
        SetValueAction = new UnityAction<T>(x => Value = x);
        _value = initValue;
        masters = null;
    }

    public bool GetInvoked()
    {
        return invoked;
    }

    public void SetInvoked(bool value)
    {
        invoked = value;
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

    public void EnslaveTo(ref IValueChangeEvent other, out bool typeMismatch)
    {        
        if (other == null) other = new ValueChangeEvent<T>();
        ValueChangeEvent<T> newMaster = other as ValueChangeEvent<T>;

        if (other is ValueChangeEvent<T>)
        {
            typeMismatch = false;
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
        {
            typeMismatch = true;
            Debug.LogError("Trying to enslave type " + GetValueType().Name + " to type " + other.GetValueType().Name);
        }
    }

    public void FreeFrom(IValueChangeEvent other, out bool typeMismatch)
    {
        if (other == null)
        {
            typeMismatch = false;
            Debug.Log("Trying to free event from null.");
            return;
        }

        if (other is ValueChangeEvent<T>)
        {
            typeMismatch = false;
            ValueChangeEvent<T> oldMaster = other as ValueChangeEvent<T>;
            oldMaster.RemoveListener(SetValueAction);
            RemoveFromMastersArray(oldMaster);
        }
        else
        {
            typeMismatch = true;
            Debug.LogError("Trying to free type " + GetValueType().Name + " from type " + other.GetValueType().Name);
        }
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