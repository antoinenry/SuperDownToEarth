using System;
using System.Reflection;

using UnityEngine;
using UnityEngine.Events;

[Serializable]
public partial class ValueChangeEvent
{
    public IValueChangeEvent runtimeEvent;

    [SerializeField] private string valueType;
    [SerializeField] private ValueChangeEventID[] masterIDs;
    
    public int MasterCount { get => masterIDs == null ? 0 : masterIDs.Length; }
    public int RuntimeMasterCount { get => runtimeEvent == null ? 0 : runtimeEvent.GetMasterCount(); }
    public Type ValueType { get => Type.GetType(valueType); }
    
    private ValueChangeEvent() { }

    public static ValueChangeEvent New<T>()
    {
        ValueChangeEvent vce = new ValueChangeEvent();
        //vce.runtimeEvent = new RuntimeValueChangeEvent<T>();
        vce.valueType = typeof(T).AssemblyQualifiedName;
        return vce;
    }

    public void SetRuntimeEvent<T>()
    {
        if (typeof(T) == ValueType)
            runtimeEvent = new RuntimeValueChangeEvent<T>();
        else
            Debug.LogWarning("Type mismatch: " + typeof(T).Name + "/" + ValueType.Name);
    }

    public void ResetRuntimeEvent()
    {
        MethodInfo resetMethod = typeof(ValueChangeEvent).GetMethod("SetRuntimeEvent", BindingFlags.Public | BindingFlags.Instance);
        resetMethod = resetMethod.MakeGenericMethod(ValueType);
        resetMethod.Invoke(this, null);
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
        else Debug.LogError("Runtime event is null. Try specifying type when adding listener.");
    }

    public void AddListener<T>(UnityAction listener)
    {
        if (runtimeEvent == null) SetRuntimeEvent<T>();
        runtimeEvent.AddListener(listener);
    }

    public void AddListener<T>(UnityAction<T> listener)
    {
        if (runtimeEvent == null) SetRuntimeEvent<T>();
        if (runtimeEvent is RuntimeValueChangeEvent<T>)
        {
            (runtimeEvent as RuntimeValueChangeEvent<T>).AddListener(listener);
        }
        else Debug.LogError("ValueChangeEvent type mismatch.");
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

    public void RemoveMastersIDs(Predicate<ValueChangeEventID> filter)
    {
        for (int i = 0; i < MasterCount; i++)
        {
            if (filter(masterIDs[i])) RemoveMasterIDAt(i);
        }
    }

    public void RemoveMasterIDAt(int index)
    {
        if (index < 0) return;

        int count = MasterCount;
        for (int i = index; i < count - 1; i++)
            masterIDs[i] = masterIDs[i + 1];

        Array.Resize(ref masterIDs, count - 1);
    }

    public void Enslave(bool enslave)
    {
        if (masterIDs == null) return;

        if(runtimeEvent == null)
        {
            Debug.LogWarning("RuntimeEvent is null.");
            return;
        }

        RemoveMastersIDs(id => id.GetValueChangeEvent() == null);
        foreach(ValueChangeEventID masterID in masterIDs)
        {
            ValueChangeEvent master = masterID.GetValueChangeEvent();

            bool typeMismatch;
            if (enslave)
                runtimeEvent.EnslaveTo(ref master.runtimeEvent, out typeMismatch);
            else
                runtimeEvent.FreeFrom(master.runtimeEvent, out typeMismatch);

            if (typeMismatch) Debug.LogWarning("Master/slave type mismatch");
        }
    }

    public void Enslave<T>(bool enslave)
    {
        if (masterIDs == null) return;
        if (runtimeEvent == null) SetRuntimeEvent<T>();
        Enslave(enslave);
    }

    public ValueChangeEventID GetMasterID(int index)
    {
        if (index < 0 || index > MasterCount)
            return ValueChangeEventID.NoID;
        else
            return masterIDs[index];
    }

    public ValueChangeEvent GetMaster(int index)
    {        
        if (index < 0 || index > MasterCount)
            return null;
        else
            return masterIDs[index].GetValueChangeEvent();
    }

    public int FindMasterIndex(ValueChangeEvent other)
    {
        if (other != null)
            for (int i = 0; i < MasterCount; i++)
                if (masterIDs[i].GetValueChangeEvent() == other)
                    return i;

        return -1;
    }

    public void AddMaster(ValueChangeEventID newMasterID)
    {
        ValueChangeEvent newMaster = newMasterID.GetValueChangeEvent();
        if (newMaster != null)
        {
            if (newMaster == this) Debug.LogWarning("Trying to add own ID as master. Master won't be added. " + newMasterID.ToString());
            else if (FindMasterIndex(newMaster) != -1) Debug.LogWarning("Trying to add ID as master but it's already registered. Master won't be added again." + newMasterID.ToString());
            else
            {
                if (runtimeEvent != null)
                    runtimeEvent.EnslaveTo(ref newMaster.runtimeEvent, out bool typeMismatch);

                Array.Resize(ref masterIDs, MasterCount + 1);
                masterIDs[MasterCount - 1] = newMasterID;
            }
        }
        else Debug.LogWarning("Trying to add a master ID but no vce found. Master won't be added. " + newMasterID.ToString());
        
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
            if (runtimeEvent != null && masterIDs[masterIndex].GetValueChangeEvent() != null)
                runtimeEvent.FreeFrom(masterIDs[masterIndex].GetValueChangeEvent().runtimeEvent, out bool typeMismatch);

            RemoveMasterIDAt(masterIndex);
        }
    }
}
