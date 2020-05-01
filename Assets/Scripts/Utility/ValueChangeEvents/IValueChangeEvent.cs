using System;

public interface IValueChangeEvent
{
    Type GetValueType();
    //string GetName();    

    void ForceInvoke();

    int GetMasterCount ();
    void EnslaveTo(ref IValueChangeEvent other);
    void FreeFrom(IValueChangeEvent other);
}
