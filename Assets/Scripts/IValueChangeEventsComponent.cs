using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IValueChangeEventsComponent
{
    void SetValueChangeEventsID();
    int GetValueChangeEvents(out ValueChangeEvent[] vces);
    int GetNewRuntimeEvents(out IValueChangeEvent[] newRuntimeEvents);
}