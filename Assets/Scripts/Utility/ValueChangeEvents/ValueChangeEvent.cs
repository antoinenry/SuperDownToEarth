using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ValueChangeEvent
{
    public IValueChangeEvent runtimeEvent;

    [SerializeField] private ValueChangeEventID ID;
    [SerializeField] private ValueChangeEventID[] mastersID;
    
    public string Name { get => ID.name; }
    public Component Component { get => ID.component; }

    public int MasterCount { get => mastersID == null ? 0 : mastersID.Length; }
    public int RuntimeMasterCount { get => runtimeEvent == null ? 0 : runtimeEvent.GetMasterCount(); }
    public Type ValueType { get => runtimeEvent == null ? (Type)Type.Missing : runtimeEvent.GetValueType(); }
    public bool Triggered
    {
        get
        {
            if (runtimeEvent == null) return false;
            else return runtimeEvent.HasChanged();
        }
        set
        {
            if (runtimeEvent == null) return;
            else runtimeEvent.HasChanged(value);
        }
    }
    
    public static ValueChangeEvent NewTriggerEvent()
    {
        ValueChangeEvent vce = new ValueChangeEvent();
        vce.runtimeEvent = new TriggerEvent();
        return vce;
    }

    public static ValueChangeEvent NewValueChangeEvent<T>()
    {
        ValueChangeEvent vce = new ValueChangeEvent();
        vce.runtimeEvent = new ValueChangeEvent<T>();
        return vce;
    }

    public void SetID(string name, Component component, int indexInComponent)
    {
        ID.name = name;
        ID.component = component;
        ID.indexInComponent = indexInComponent;
    }

    public void Invoke()
    {
        if(runtimeEvent != null)
            runtimeEvent.Trigger();
    }

    public T GetValue<T>()
    {
        if (runtimeEvent != null && runtimeEvent is ValueChangeEvent<T>) return (runtimeEvent as ValueChangeEvent<T>).Value;
        if (runtimeEvent is ValueChangeEvent<T> == false) Debug.LogError("ValueChangeEvent type mismatch.");
        return default(T);        
    }

    public void SetValue<T>(T value)
    {
        if (runtimeEvent != null && runtimeEvent is ValueChangeEvent<T>) (runtimeEvent as ValueChangeEvent<T>).Value = value;
        if (runtimeEvent is ValueChangeEvent<T> == false) Debug.LogError("ValueChangeEvent type mismatch.");
    }

    public void AddListener(UnityAction listener)
    {
        if (runtimeEvent == null) runtimeEvent = new TriggerEvent();
        if (runtimeEvent is TriggerEvent) (runtimeEvent as TriggerEvent).AddListener(listener);
        else Debug.LogError("ValueChangeEvent type mismatch.");
    }

    public void AddListener<T>(UnityAction<T> listener)
    {
        if (runtimeEvent == null) runtimeEvent = new ValueChangeEvent<T>();
        if (runtimeEvent is ValueChangeEvent<T>) (runtimeEvent as ValueChangeEvent<T>).AddListener(listener);
        else Debug.LogError("ValueChangeEvent type mismatch.");
    }

    public void RemoveListener(UnityAction listener)
    {
        if (runtimeEvent == null) return;
        if (runtimeEvent is TriggerEvent) (runtimeEvent as TriggerEvent).RemoveListener(listener);
        else Debug.LogError("ValueChangeEvent type mismatch.");
    }

    public void RemoveListener<T>(UnityAction<T> listener)
    {
        if (runtimeEvent == null) return;
        if (runtimeEvent is ValueChangeEvent<T>) (runtimeEvent as ValueChangeEvent<T>).RemoveListener(listener);
        else Debug.LogError("ValueChangeEvent type mismatch.");
    }

    public void Enslave()
    {
        if (MasterCount > 0)
        {
            foreach (ValueChangeEventID masterID in mastersID)
            {
                ValueChangeEvent master = masterID.ValueChangeEvent;
                if (master != null) runtimeEvent.EnslaveTo(master);
            }
        }
    }

    public void SetFree()
    {
        if (MasterCount > 0)
        {
            foreach (ValueChangeEventID masterID in mastersID)
            {
                ValueChangeEvent master = masterID.ValueChangeEvent;
                if (master != null) runtimeEvent.FreeFrom(master);
            }
        }
    }

    public ValueChangeEvent GetMaster(int index)
    {
        if (mastersID == null || index < 0 || index > mastersID.Length)
            return null;
        else
            return mastersID[index].ValueChangeEvent;
    }

    public int FindMasterIndex(ValueChangeEvent other)
    {
        if (mastersID != null && other != null)
            for (int i = 0; i < MasterCount; i++)
                if (mastersID[i].ValueChangeEvent == other) return i;

        return -1;
    }

    public void AddMaster(ValueChangeEvent newMaster)
    {
        if(newMaster != null && newMaster != this && FindMasterIndex(newMaster) == -1)
        {
            if(runtimeEvent != null)
                runtimeEvent.EnslaveTo(newMaster);

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
            if (runtimeEvent != null)
                runtimeEvent.FreeFrom(mastersID[masterIndex].ValueChangeEvent);

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
