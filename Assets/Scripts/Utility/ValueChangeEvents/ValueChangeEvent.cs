using System;

using UnityEngine;
using UnityEngine.Events;

[Serializable]
public partial class ValueChangeEvent
{
    public IValueChangeEvent runtimeEvent;

    [SerializeField] private ValueChangeEvent[] masters;

    public string Name { get; private set; }
    public int MasterCount { get => masters == null ? 0 : masters.Length; }
    public int RuntimeMasterCount { get => runtimeEvent == null ? 0 : runtimeEvent.GetMasterCount(); }
    public Type ValueType { get => runtimeEvent == null ? null : runtimeEvent.GetValueType(); }
    
    private ValueChangeEvent() { }

    public void SetID(string name, Component component, int index)
    {
        Name = name;
    }

    public static ValueChangeEvent New<T>(string name = "NO NAME")
    {
        ValueChangeEvent vce = new ValueChangeEvent();
        vce.runtimeEvent = new RuntimeValueChangeEvent<T>();
        vce.Name = name;
        return vce;
    }

    public void ResetRuntimeEvent<T>()
    {
        runtimeEvent = new RuntimeValueChangeEvent<T>();
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

    public T Get<T>()
    {
        if (runtimeEvent != null)
        {
            if (runtimeEvent is RuntimeValueChangeEvent<T>) return (runtimeEvent as RuntimeValueChangeEvent<T>).Value;
            else Debug.LogError("ValueChangeEvent type mismatch.");
        }
        return default(T);        
    }

    public void Set<T>(T value)
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
        if (masters == null) return;

        if(runtimeEvent == null)
        {
            Debug.LogWarning("RuntimeEvent is null.");
            return;
        }

        foreach(ValueChangeEvent master in masters)
        {
            if (master == null)
            {
                Debug.LogWarning("Master is null");
                continue;
            }

            bool typeMismatch;
            if (enslave)
                runtimeEvent.EnslaveTo(ref master.runtimeEvent, out typeMismatch);
            else
                runtimeEvent.FreeFrom(master.runtimeEvent, out typeMismatch);

            if (typeMismatch) Debug.LogWarning("Master/slave type mismatch");
        }
    }

    public ValueChangeEvent GetMaster(int index)
    {
        if (masters == null || index < 0 || index > masters.Length)
            return null;
        else
            return masters[index];
    }

    public int FindMasterIndex(ValueChangeEvent other)
    {
        if (masters != null && other != null)
            for (int i = 0; i < MasterCount; i++)
                if (masters[i] == other)
                    return i;

        return -1;
    }

    public void AddMaster(ValueChangeEvent newMaster)
    {
        if(newMaster != null && newMaster != this && FindMasterIndex(newMaster) == -1)
        {
            if(runtimeEvent != null)
                runtimeEvent.EnslaveTo(ref newMaster.runtimeEvent, out bool typeMismatch);
            
            Array.Resize(ref masters, MasterCount + 1);
            masters[MasterCount-1] = newMaster;
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
            if (runtimeEvent != null && masters[masterIndex] != null)
                runtimeEvent.FreeFrom(masters[masterIndex].runtimeEvent, out bool typeMismatch);

            for (int i = masterIndex; i < MasterCount - 1; i++)
                masters[i] = masters[i + 1];
            Array.Resize(ref masters, MasterCount-1);
        }
    }

    public override string ToString()
    {
        if (ValueType == null)
            return Name + " (trigger)";
        else
            return Name + " (" + ValueType.Name + ")";
    }
}
