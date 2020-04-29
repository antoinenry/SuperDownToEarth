using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Vehicle : PhysicalBody, IEventsHubElement
{
    [Header("Vehicle")]
    public Transform seat;
    public Transform exit;
    public float exitForce;
    public float exitAnimationDelay;

    public enum ExposedEvents { isFull }
    public ValueChangeEvent<bool> IsFull = new ValueChangeEvent<bool>();
    public Body BodyInside { get; private set; }

    protected override void Init()
    {
        base.Init();
        SetBodyInside(BodyInside);
    }

    public void Eject()
    {
        SetBodyInside(null);
    }

    public void SetBodyInside(Body body)
    {
        BodyInside = body;

        if (body == null)
        {
            IsFull.Value = false;
            isFree = true;
            pilotableConfig.Enabled = Pilotable.PartEnabled.AllDisabled;
        }
        else
        {
            IsFull.Value = true;
            isFree = false;
            pilotableConfig.Enabled = Pilotable.PartEnabled.AllEnabled;
        }
    }

    public bool GetValueChangeEvent(int index, out IValueChangeEvent iValueChangeEvent)
    {
        switch (index)
        {
            case (int)ExposedEvents.isFull:
                iValueChangeEvent = IsFull;
                return true;
        }

        iValueChangeEvent = null;
        return false;
    }

    public void GetValueChangeEventsNamesAndTypes(out string[] names, out Type[] types)
    {
        names = Enum.GetNames(typeof(ExposedEvents));
        types = new Type[] { typeof(bool) };
    }

    public int GetValueChangeEventIndex(string vceName)
    {
        List<string> names = new List<string>(Enum.GetNames(typeof(ExposedEvents)));
        return names.IndexOf(vceName);
    }
}