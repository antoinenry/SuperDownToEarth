using System;
using UnityEditor;

public interface IValueChangeEvent
{
    Type GetValueType();
    //string GetName();    

    void Trigger();

    int GetMasterCount ();
    void EnslaveTo(ValueChangeEvent other);
    void FreeFrom(ValueChangeEvent other);
}
