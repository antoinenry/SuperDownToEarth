using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IValueChangeEventsComponent
{
    int GetValueChangeEvents(out ValueChangeEvent[] vces);
    ValueChangeEvent GetValueChangeEventByName(string vceName);
    int SetValueChangeEventsID();
    void EnslaveValueChangeEvents(bool enslave);
}