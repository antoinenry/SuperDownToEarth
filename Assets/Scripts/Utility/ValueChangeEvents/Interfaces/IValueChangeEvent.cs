using System;
using UnityEngine.Events;

public interface IValueChangeEvent
{
    Type GetValueType();
    //string GetName();    

    void InvokeEvent();
    void AddListener(UnityAction listener);
    void RemoveListener(UnityAction listener);


    int GetMasterCount ();
    void EnslaveTo(ref IValueChangeEvent other, out bool typeMismatch);
    void FreeFrom(IValueChangeEvent other, out bool typeMismatch);
}
