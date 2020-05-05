using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatGroundProbe : BodyPart, IValueChangeEventsComponent
{
    public enum Flatness { Flat, Hole, Point, BackDrop, FrontDrop, NoGround, Other}    

    public SimpleTrigger2D center;
    public SimpleTrigger2D frontUp;
    public SimpleTrigger2D frontDown;
    public SimpleTrigger2D backUp;
    public SimpleTrigger2D backDown;

    public bool CenterCollision { get => center != null && center.IsTriggered.Get<bool>(); }
    public bool FrontDownCollision { get => frontDown != null && frontDown.IsTriggered.Get<bool>(); }
    public bool FrontUpCollision { get => frontUp != null && frontUp.IsTriggered.Get<bool>(); }
    public bool BackDownCollision { get => backDown != null && backDown.IsTriggered.Get<bool>(); }
    public bool BackUpCollision { get => backUp != null && backUp.IsTriggered.Get<bool>(); }

    public ValueChangeEvent GroundFlatness = ValueChangeEvent.New<int>();

    public int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        vces = new ValueChangeEvent[] { GroundFlatness };
        return vces.Length;
    }

    public int SetValueChangeEventsID()
    {
        GroundFlatness.SetID("GroundFlatness", this, 0);
        return 1;
    }

    public void EnslaveValueChangeEvents(bool enslave)
    {
        GroundFlatness.Enslave(enslave);
    }

    private void Start()
    {
        OnTriggerEvent();

        if (center != null) center.IsTriggered.AddListener<bool>(OnTriggerEvent);
        if (frontUp != null) frontUp.IsTriggered.AddListener<bool>(OnTriggerEvent);
        if (frontDown != null) frontDown.IsTriggered.AddListener<bool>(OnTriggerEvent);
        if (backUp != null) backUp.IsTriggered.AddListener<bool>(OnTriggerEvent);
        if (backDown != null) backDown.IsTriggered.AddListener<bool>(OnTriggerEvent);
    }

    private void OnTriggerEvent(bool triggered = false)
    {
        if (!CenterCollision)// && !FrontUpCollision && !BackDownCollision)
            GroundFlatness.Set((int)Flatness.NoGround);

        else if (FrontUpCollision || BackUpCollision)
            GroundFlatness.Set((int)Flatness.Hole);

        else if (FrontDownCollision && BackDownCollision)
            GroundFlatness.Set((int)Flatness.Flat);

        else if (!FrontDownCollision && !BackDownCollision)
            GroundFlatness.Set((int)Flatness.Point);

        else if (FrontDownCollision && !BackDownCollision)
            GroundFlatness.Set((int)Flatness.BackDrop);

        else if (!FrontDownCollision && BackDownCollision)
            GroundFlatness.Set((int)Flatness.FrontDrop);

        else GroundFlatness.Set((int)Flatness.Other);
    }
}