using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : UnityEvent, IValueChangeEvent
{
    public bool triggered;

    private TriggerEvent[] masters;

    //public string Name { get; private set; }
    public UnityAction TriggerAction { get; private set; }

    public TriggerEvent(string name = "NewTriggerEvent", bool initTrigger = false)
    {
        //this.Name = name;
        TriggerAction = new UnityAction(Trigger);
        triggered = initTrigger;
        masters = null;
    }

    public void Trigger()
    {
        Invoke();
        triggered = true;        
    }

    public bool HasChanged()
    {
        return triggered;
    }

    public void HasChanged(bool set)
    {
        triggered = set;
    }

    public Type GetValueType()
    {
        return null;
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
            Debug.Log("Trying to enslave trigger to null");
            return;
        }

        if (other.runtimeEvent == null)
            other.runtimeEvent = new TriggerEvent();

        if (other.runtimeEvent is TriggerEvent)
        {
            TriggerEvent newMaster = other.runtimeEvent as TriggerEvent;
            int currentMasterCount = GetMasterCount();

            if (currentMasterCount == 0)
                masters = new TriggerEvent[] { newMaster };
            else
            {
                foreach (TriggerEvent oldMasters in masters)
                    if (newMaster == oldMasters) return;

                Array.Resize(ref masters, currentMasterCount + 1);
                masters[currentMasterCount] = newMaster;
            }

            newMaster.AddListener(TriggerAction);
        }
        else
            Debug.LogError("Trying to enslave trigger to type " + other.ValueType.Name);
    }

    public void FreeFrom(ValueChangeEvent other)
    {
        if (other == null || other.runtimeEvent == null)
        {
            Debug.LogError("Trying to free trigger from null.");
            return;
        }

        if (other.runtimeEvent is TriggerEvent)
        {
            TriggerEvent oldMaster = other.runtimeEvent as TriggerEvent;
            oldMaster.RemoveListener(TriggerAction);
            RemoveFromMastersArray(oldMaster);
        }
        else
            Debug.LogError("Trying to free trigger from type " + other.ValueType.Name);
    }

    private void RemoveFromMastersArray(TriggerEvent remove)
    {
        if (masters != null)
        {
            List<TriggerEvent> clean = new List<TriggerEvent>(masters);
            if (clean.RemoveAll(m => m == remove) > 0) masters = clean.ToArray();

            if (masters.Length == 0) masters = null;
        }
    }
}
