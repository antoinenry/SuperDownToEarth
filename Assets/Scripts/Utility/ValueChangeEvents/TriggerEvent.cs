/*using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : UnityEvent, IValueChangeEvent
{
    private TriggerEvent[] masters;
    private bool invoked;

    //public string Name { get; private set; }
    public UnityAction TriggerAction { get; private set; }

    public TriggerEvent(string name = "NewTriggerEvent", bool initTrigger = false)
    {
        //this.Name = name;
        TriggerAction = new UnityAction(ForceInvoke);
        masters = null;
    }

    public void ForceInvoke()
    {
        Invoke();
        invoked = true;
    }

    public bool GetInvoked()
    {
        return invoked;
    }

    public void SetInvoked(bool value)
    {
        invoked = value;
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

    public void EnslaveTo(ref IValueChangeEvent other, out bool typeMismatch)
    {
        if (other == null) other = new TriggerEvent();
        TriggerEvent newMaster = other as TriggerEvent;

        if (other is TriggerEvent)
        {
            typeMismatch = false;

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
        {
            Debug.LogWarning("Trying to enslave trigger to type " + other.GetValueType().Name);
            typeMismatch = true;
        }
    }

    public void FreeFrom(IValueChangeEvent other, out bool typeMismatch)
    {
        if (other == null)
        {
            Debug.LogWarning("Trying to free trigger from null.");
            typeMismatch = false;
            return;
        }

        if (other is TriggerEvent)
        {
            TriggerEvent oldMaster = other as TriggerEvent;
            oldMaster.RemoveListener(TriggerAction);
            RemoveFromMastersArray(oldMaster);
            typeMismatch = false;
        }
        else
        {
            Debug.LogWarning("Trying to free trigger from type " + other.GetValueType().Name);
            typeMismatch = true;
        }
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
*/