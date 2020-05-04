using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[Serializable]
public partial class ValueChangeEvent
{    
    public IValueChangeEvent runtimeEvent;

    [SerializeField] private ValueChangeEventID ID;
    [SerializeField] private ValueChangeEventID[] mastersID;

    public string Name { get => ID.name; }
    public Component Component { get => ID.component; }

    public int MasterCount { get => mastersID == null ? 0 : mastersID.Length; }
    public int RuntimeMasterCount { get => runtimeEvent == null ? 0 : runtimeEvent.GetMasterCount(); }
    public Type ValueType { get => runtimeEvent == null ? (Type)Type.Missing : runtimeEvent.GetValueType(); }
    

    private ValueChangeEvent() { }
    
    public static ValueChangeEvent New<T>()
    {
        ValueChangeEvent vce = new ValueChangeEvent();
        vce.runtimeEvent = new RuntimeValueChangeEvent<T>();
        return vce;
    }

    public void ResetRuntimeEvent<T>()
    {
        runtimeEvent = new RuntimeValueChangeEvent<T>();
    }

    public void SetID(string name, Component component, int indexInComponent)
    {
        ID.name = name;
        ID.component = component;
        ID.indexInComponent = indexInComponent;
    }

    public void Invoke()
    {
        if (runtimeEvent != null)
            runtimeEvent.InvokeEvent();
    }

    public bool IsValueType<T>()
    {
        return ValueType == typeof(T);
    }

    public T GetValue<T>()
    {
        if (runtimeEvent != null)
        {
            if (runtimeEvent is RuntimeValueChangeEvent<T>) return (runtimeEvent as RuntimeValueChangeEvent<T>).Value;
            else Debug.LogError("ValueChangeEvent type mismatch.");
        }
        return default(T);        
    }

    public void SetValue<T>(T value)
    {
        if (runtimeEvent != null)
        {
            if (runtimeEvent is RuntimeValueChangeEvent<T>) (runtimeEvent as RuntimeValueChangeEvent<T>).Value = value;
            else Debug.LogError("ValueChangeEvent type mismatch.");
        }
    }

    public void AddListener(UnityAction listener)
    {
        if (runtimeEvent != null) runtimeEvent.AddListener(listener);
        else Debug.LogError("Runtime event is null.");
    }

    public void AddListener<T>(UnityAction<T> listener)
    {
        if (runtimeEvent != null)
        {
            if (runtimeEvent is RuntimeValueChangeEvent<T>) (runtimeEvent as RuntimeValueChangeEvent<T>).AddListener(listener);
            else Debug.LogError("ValueChangeEvent type mismatch.");
        }
        else Debug.LogError("Runtime event is null.");
    }

    public void RemoveListener(UnityAction listener)
    {
        if (runtimeEvent != null) runtimeEvent.RemoveListener(listener);
        else Debug.LogError("Runtime event is null.");
    }

    public void RemoveListener<T>(UnityAction<T> listener)
    {
        if (runtimeEvent == null) return;
        if (runtimeEvent is RuntimeValueChangeEvent<T>) (runtimeEvent as RuntimeValueChangeEvent<T>).RemoveListener(listener);
        else Debug.LogError("ValueChangeEvent type mismatch.");
    }

    public void Enslave(bool enslave)
    {
        if(runtimeEvent == null)
        {
            Debug.LogWarning("RuntimeEvent is null.");
            return;
        }

        for (int i = 0; i < MasterCount; i++)
        {
            bool removeMasterId = ValueChangeEventID.GetValueChangeEvent(mastersID[i], out ValueChangeEvent master);

            if (removeMasterId == true)
            {
                if (enslave)
                    runtimeEvent.EnslaveTo(ref master.runtimeEvent, out removeMasterId);
                else
                    runtimeEvent.FreeFrom(master.runtimeEvent, out removeMasterId);
            }
            else
                Debug.LogWarning(master == null ? "Master is null." : "ID doesn't match any event.");

            if (removeMasterId == true)
            {
                Debug.LogWarning("Removed master " + mastersID[i].ToString());
                RemoveMasterAt(i);
            }
        }
    }

    public ValueChangeEvent GetMaster(int index)
    {
        if (mastersID == null || index < 0 || index > mastersID.Length)
            return null;
        else
        {
            ValueChangeEventID.GetValueChangeEvent(mastersID[index], out ValueChangeEvent mastervce);
            return mastervce;
        }
    }

    public int FindMasterIndex(ValueChangeEvent other)
    {
        if (mastersID != null && other != null)
            for (int i = 0; i < MasterCount; i++)
                if (ValueChangeEventID.GetValueChangeEvent(mastersID[i], out ValueChangeEvent mastervce) && mastervce == other)
                    return i;

        return -1;
    }

    public void AddMaster(ValueChangeEvent newMaster)
    {
        if(newMaster != null && newMaster != this && FindMasterIndex(newMaster) == -1)
        {
            if(runtimeEvent != null)
                runtimeEvent.EnslaveTo(ref newMaster.runtimeEvent, out bool typeMismatch);
            
            Array.Resize(ref mastersID, MasterCount + 1);
            mastersID[MasterCount-1] = newMaster.ID;
        }
    }

    public void RemoveMaster(ValueChangeEvent oldMaster)
    {
        int masterIndex = FindMasterIndex(oldMaster);
        if (masterIndex != -1)
            RemoveMasterAt(masterIndex);
    }

    public void RemoveMasterAt(int masterIndex)
    {
        if (masterIndex >= 0 && masterIndex < MasterCount)
        {
            if (runtimeEvent != null && ValueChangeEventID.GetValueChangeEvent(mastersID[masterIndex], out ValueChangeEvent mastervce))
                runtimeEvent.FreeFrom(mastervce.runtimeEvent, out bool typeMismatch);

            for (int i = masterIndex; i < MasterCount - 1; i++)
                mastersID[i] = mastersID[i + 1];
            Array.Resize(ref mastersID, MasterCount-1);
        }
    }

    public override string ToString()
    {
        if (ValueType == null)
            return ID.name + "(trigger)";
        else
            return ID.name + "(" + ValueType.Name + ")";
    }
}
