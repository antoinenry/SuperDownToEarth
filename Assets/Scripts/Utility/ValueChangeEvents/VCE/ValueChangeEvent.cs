using System;
using System.Reflection;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace VCE
{
    [Serializable]
    public class ValueChangeEvent
    {
        [NonSerialized] public bool inspectorHighlight;
        public bool inspectorUnfold;

        public readonly Type ValueType;

        private FieldInfo valueField;
        private IEventGeneric changeEvent;
        [SerializeField] private ValueChangeEventID[] masters;

        private UnityAction trigger;
        private object setValue;

        public Action<object> listenerRemoved;

        public ValueChangeEvent()
        {
            valueField = GetType().GetField("value", BindingFlags.NonPublic | BindingFlags.Instance);

            if (valueField == null)
            {
                ValueType = null;
                changeEvent = new TriggerEvent();
                setValue = null;
            }
            else
            {
                ValueType = valueField.FieldType;
                typeof(ValueChangeEvent).GetMethod("CreateValueEvent", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(ValueType).Invoke(this, null);
                typeof(ValueChangeEvent).GetMethod("CreateSetValueAction", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(ValueType).Invoke(this, null);
            }

            trigger = new UnityAction(Trigger);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        ~ValueChangeEvent()
        {
            RemoveAllListeners();
        }

        private void CreateValueEvent<T>()
        {
            changeEvent = new ValueEvent<T>();
        }

        private void CreateSetValueAction<T>()
        {
            setValue = new UnityAction<T>(SetValue);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ListenToMasters();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void Trigger()
        {
            if (ValueType != null) changeEvent.Invoke(Value);
            changeEvent.Invoke();
        }

        public object Value
        {
            get => (valueField == null) ? null : valueField.GetValue(this);

            set
            {
                if (ValueType == null)
                {
                    Debug.LogWarning("Trying to set a trigger's value.");
                    return;
                }

                object currentValue = Value;
                if ((currentValue == null && value != null) || !currentValue.Equals(value))
                {
                    valueField.SetValue(this, value);
                    Trigger();
                }
            }
        }

        private void SetValue<T>(T value)
        {
            Value = value;
        }



        public void AddTriggerListener(UnityAction listener, bool invokeWhenAdded = true)
        {
            if (listener == null) return;
            changeEvent.AddListener(listener);
            if (invokeWhenAdded) listener.Invoke();
        }

        public void AddValueListener<T>(UnityAction<T> listener, bool invokeWhenAdded = true)
        {
            if (listener == null) return;

            if (ValueType == typeof(T))
            {
                changeEvent.AddListener(listener);
                if (invokeWhenAdded) listener.Invoke((T)Value);
            }
            else Debug.LogError("Value/listener type mismatch : " + ValueType + "/" + typeof(T));
        }

        public void AddValueListenerObject(object listener, bool invokeWhenAdded = true)
        {
            MethodInfo addListener = typeof(ValueChangeEvent).GetMethod("AddValueListenerObject", BindingFlags.NonPublic | BindingFlags.Instance);
            addListener = addListener.MakeGenericMethod(ValueType);
            addListener.Invoke(this, new object[] { listener, invokeWhenAdded });
        }

        private void AddValueListenerObject<T>(object listener, bool invokeWhenAdded)
        {
            AddValueListener(listener as UnityAction<T>, invokeWhenAdded);
        }



        public void RemoveTriggerListener(UnityAction listener)
        {
            if (listener == null) return;
            changeEvent.RemoveListener(listener);
        }

        public void RemoveValueListener<T>(UnityAction<T> listener)
        {
            if (listener == null) return;

            if (ValueType == typeof(T))
                changeEvent.RemoveListener(listener);
            else
                Debug.LogError("Value/listener type mismatch : " + ValueType + "/" + typeof(T));
        }

        public void RemoveValueListenerObject(object listener)
        {
            MethodInfo removeListener = typeof(ValueChangeEvent).GetMethod("RemoveValueListenerObject", BindingFlags.NonPublic | BindingFlags.Instance);
            removeListener = removeListener.MakeGenericMethod(ValueType);
            removeListener.Invoke(this, new object[] { listener });
        }

        private void RemoveValueListenerObject<T>(object listener)
        {
            RemoveValueListener(listener as UnityAction<T>);
        }

        public void RemoveAllListeners()
        {
            if (changeEvent != null) changeEvent.RemoveAllListeners();
        }



        public int MasterCount { get => masters == null ? 0 : masters.Length; }

        public void ListenToMasters() { for (int i = 0; i < MasterCount; i++) ListenToMasterAt(i); }

        public void IgnoreMasters() { for (int i = 0; i < MasterCount; i++) IgnoreMasterAt(i); }

        private void ListenToMasterAt(int index)
        {
            ValueChangeEvent masterVCE = masters[index].ValueChangeEvent;
            if (masterVCE != null)
            {
                if (ValueType == null)
                    masterVCE.AddTriggerListener(trigger);
                else
                    masterVCE.AddValueListenerObject(setValue);
            }
        }

        private void IgnoreMasterAt(int index)
        {
            ValueChangeEvent masterVCE = masters[index].ValueChangeEvent;
            if (masterVCE != null)
            {
                if (ValueType == null)
                    masterVCE.RemoveTriggerListener(trigger);
                else
                    masterVCE.RemoveValueListenerObject(setValue);
            }
        }

        public ValueChangeEventID GetMaster(int index)
        {
            return (index >= 0 && index < MasterCount) ? masters[index] : ValueChangeEventID.None;
        }

        public int AddMaster(ValueChangeEventID newMasterID)
        {
            ValueChangeEvent masterVCE = newMasterID.ValueChangeEvent;
            if (masterVCE != null)
            {
                if (masterVCE.ValueType != null & masterVCE.ValueType != ValueType)
                    return 3;

                int masterCount = MasterCount;
                for (int i = 0; i < masterCount; i++)
                    if (masters[i].Equals(newMasterID)) return 1;

                Array.Resize(ref masters, masterCount + 1);
                masters[masterCount] = newMasterID;
                ListenToMasterAt(masterCount);
                return 0;
            }
            return 2;
        }

        public int RemoveMaster(ValueChangeEventID oldMasterID)
        {
            ValueChangeEvent masterVCE = oldMasterID.ValueChangeEvent;
            bool typeMismatch = false;

            int masterCount = MasterCount;
            for (int i = 0; i < masterCount; i++)
                if (masters[i].Equals(oldMasterID))
                {
                    IgnoreMasterAt(i);
                    RemoveMasterAt(i);
                    return 0;
                }
            return typeMismatch ? 3 : 1;
        }

        public int RemoveMasterAt(int index)
        {
            int masterCount = MasterCount;

            if (index >= 0 && index < masterCount)
            {
                masterCount--;
                for (int i = index; i < masterCount; i++)
                    masters[i] = masters[i + 1];
                Array.Resize(ref masters, masterCount);
                return 0;
            }
            else return 1;
        }
    }
}