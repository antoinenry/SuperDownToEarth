using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IValueChangeEventsComponent
{
    int GetValueChangeEvents(out ValueChangeEvent[] vces);
    void SetValueChangeEventsID();
    //int GetNewRuntimeEvents(out IValueChangeEvent[] newRuntimeEvents);
}