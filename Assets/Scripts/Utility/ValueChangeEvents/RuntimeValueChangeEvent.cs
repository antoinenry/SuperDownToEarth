using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public partial class ValueChangeEvent
{
    private class RuntimeValueChangeEvent<T> : UnityEvent<T>, IValueChangeEvent
    {
        private T _value;
        private RuntimeValueChangeEvent<T>[] masters;

        private UnityEvent OnInvoke;

        public UnityAction InvokeEventAction { get; private set; }
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
                        InvokeEvent();
                    }

                    return;
                }

                if (_value.Equals(value) == false)
                {
                    _value = value;
                    InvokeEvent();
                }
            }
        }

        public RuntimeValueChangeEvent()
        {
            OnInvoke = new UnityEvent();
            InvokeEventAction = new UnityAction(InvokeEvent);
            SetValueAction = new UnityAction<T>(x => Value = x);

            masters = null;
        }

        public void InvokeEvent()
        {
            Invoke(_value);
            OnInvoke.Invoke();
        }

        public Type GetValueType()
        {
            return typeof(T);
        }

        public void AddListener(UnityAction listener)
        {
            OnInvoke.AddListener(listener);
        }

        public void RemoveListener(UnityAction listener)
        {
            OnInvoke.RemoveListener(listener);
        }

        public int GetMasterCount()
        {
            RemoveFromMastersArray(null);
            return masters == null ? 0 : masters.Length;
        }

        public void EnslaveTo(ref IValueChangeEvent other, out bool typeMismatch)
        {
            if (other == null) other = new RuntimeValueChangeEvent<T>();
            RuntimeValueChangeEvent<T> newMaster = other as RuntimeValueChangeEvent<T>;

            if (other is RuntimeValueChangeEvent<T>)
            {
                typeMismatch = false;
                int currentMasterCount = GetMasterCount();

                if (currentMasterCount == 0)
                    masters = new RuntimeValueChangeEvent<T>[] { newMaster };
                else
                {
                    foreach (RuntimeValueChangeEvent<T> oldMasters in masters)
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

            if (other is RuntimeValueChangeEvent<T>)
            {
                typeMismatch = false;
                RuntimeValueChangeEvent<T> oldMaster = other as RuntimeValueChangeEvent<T>;
                oldMaster.RemoveListener(SetValueAction);
                RemoveFromMastersArray(oldMaster);
            }
            else
            {
                typeMismatch = true;
                Debug.LogError("Trying to free type " + GetValueType().Name + " from type " + other.GetValueType().Name);
            }
        }

        private void RemoveFromMastersArray(RuntimeValueChangeEvent<T> remove)
        {
            if (masters != null)
            {
                List<RuntimeValueChangeEvent<T>> clean = new List<RuntimeValueChangeEvent<T>>(masters);
                if (clean.RemoveAll(m => m == remove) > 0) masters = clean.ToArray();

                if (masters.Length == 0) masters = null;
            }
        }
    }
}