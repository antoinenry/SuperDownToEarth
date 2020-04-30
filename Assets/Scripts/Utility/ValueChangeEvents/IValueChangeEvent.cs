using System;
using UnityEditor;

public interface IValueChangeEvent
{
    Type GetValueType();
    //string GetName();    

    void Trigger();
    bool HasChanged();
    void HasChanged(bool setFalse);

    int GetMasterCount ();
    void EnslaveTo(ValueChangeEvent other);
    void FreeFrom(ValueChangeEvent other);
}
