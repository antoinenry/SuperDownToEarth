using System;

public interface IValueChangeEvent
{
    Type GetValueType();
    //string GetName();    

    void ForceInvoke();
    bool GetInvoked();
    void SetInvoked(bool value);

    int GetMasterCount ();
    void EnslaveTo(ref IValueChangeEvent other, out bool typeMismatch);
    void FreeFrom(IValueChangeEvent other, out bool typeMismatch);
}
