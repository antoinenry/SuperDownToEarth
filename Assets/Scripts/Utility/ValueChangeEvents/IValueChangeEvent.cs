using System;

public interface IValueChangeEvent
{
    Type GetValueType();
    //string GetName();    

    void ForceInvoke();

    int GetMasterCount ();
    void EnslaveTo(ValueChangeEvent other);
    void FreeFrom(ValueChangeEvent other);
}
