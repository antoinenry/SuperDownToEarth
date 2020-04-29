using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public interface IEventsHubElement
{
    bool GetValueChangeEvent(int index, out IValueChangeEvent iValueChangeEvent);

    void GetValueChangeEventsNamesAndTypes(out string[] names, out Type[] types);

    int GetValueChangeEventIndex(string vceName);
}
