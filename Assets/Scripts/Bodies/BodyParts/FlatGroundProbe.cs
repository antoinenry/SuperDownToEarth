using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatGroundProbe : BodyPart, IEventsHubElement
{
    public enum Flatness { Flat, Hole, Point, BackDrop, FrontDrop, NoGround, Other}

    public enum ExposedEvents { groundFlatness }
    public ValueChangeEvent<int> GroundFlatness = new ValueChangeEvent<int>();

    public SimpleTrigger2D center;
    public SimpleTrigger2D frontUp;
    public SimpleTrigger2D frontDown;
    public SimpleTrigger2D backUp;
    public SimpleTrigger2D backDown;

    public bool CenterCollision { get => center != null && center.IsTriggered.Value; }
    public bool FrontDownCollision { get => frontDown != null && frontDown.IsTriggered.Value; }
    public bool FrontUpCollision { get => frontUp != null && frontUp.IsTriggered.Value; }
    public bool BackDownCollision { get => backDown != null && backDown.IsTriggered.Value; }
    public bool BackUpCollision { get => backUp != null && backUp.IsTriggered.Value; }

    private void Awake()
    {
        //GroundFlatness = new ValueChangeEvent<int>();
    }

    private void Start()
    {
        OnTriggerEvent();

        if (center != null) center.IsTriggered.AddListener(OnTriggerEvent);
        if (frontUp != null) frontUp.IsTriggered.AddListener(OnTriggerEvent);
        if (frontDown != null) frontDown.IsTriggered.AddListener(OnTriggerEvent);
        if (backUp != null) backUp.IsTriggered.AddListener(OnTriggerEvent);
        if (backDown != null) backDown.IsTriggered.AddListener(OnTriggerEvent);
    }

    private void OnTriggerEvent(bool triggered = false)
    {
        if (!CenterCollision)// && !FrontUpCollision && !BackDownCollision)
            GroundFlatness.Value = (int)Flatness.NoGround;

        else if (FrontUpCollision || BackUpCollision)
            GroundFlatness.Value = (int)Flatness.Hole;

        else if (FrontDownCollision && BackDownCollision)
            GroundFlatness.Value = (int)Flatness.Flat;

        else if (!FrontDownCollision && !BackDownCollision)
            GroundFlatness.Value = (int)Flatness.Point;

        else if (FrontDownCollision && !BackDownCollision)
            GroundFlatness.Value = (int)Flatness.BackDrop;

        else if (!FrontDownCollision && BackDownCollision)
            GroundFlatness.Value = (int)Flatness.FrontDrop;

        else GroundFlatness.Value = (int)Flatness.Other;
    }

    public bool GetValueChangeEvent(int index, out IValueChangeEvent iValueChangeEvent)
    {
        switch (index)
        {
            case (int)ExposedEvents.groundFlatness:
                iValueChangeEvent = GroundFlatness;
                return true;
        }

        iValueChangeEvent = null;
        return false;
    }

    public void GetValueChangeEventsNamesAndTypes(out string[] names, out Type[] types)
    {
        names = Enum.GetNames(typeof(ExposedEvents));
        types = new Type[] { typeof(int) };
    }

    public int GetValueChangeEventIndex(string vceName)
    {
        List<string> names = new List<string>(Enum.GetNames(typeof(ExposedEvents)));
        return names.IndexOf(vceName);
    }
}