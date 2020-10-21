using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatGroundProbe : BodyPart
{
    public enum Flatness { Flat, Hole, Point, BackDrop, FrontDrop, NoGround, Other}    

    public SimpleTrigger2D center;
    public SimpleTrigger2D frontUp;
    public SimpleTrigger2D frontDown;
    public SimpleTrigger2D backUp;
    public SimpleTrigger2D backDown;

    public bool CenterCollision { get => center != null && (bool)center.IsTriggered.Value; }
    public bool FrontDownCollision { get => frontDown != null && (bool)frontDown.IsTriggered.Value; }
    public bool FrontUpCollision { get => frontUp != null && (bool)frontUp.IsTriggered.Value; }
    public bool BackDownCollision { get => backDown != null && (bool)backDown.IsTriggered.Value; }
    public bool BackUpCollision { get => backUp != null && (bool)backUp.IsTriggered.Value; }

    public IntChangeEvent GroundFlatness;
    public BoolChangeEvent groundIsFlat;

    private void Start()
    {
        OnTrigger2DEvent();

        if (center != null) center.IsTriggered.AddValueListener<bool>(OnTrigger2DEvent);
        if (frontUp != null) frontUp.IsTriggered.AddValueListener<bool>(OnTrigger2DEvent);
        if (frontDown != null) frontDown.IsTriggered.AddValueListener<bool>(OnTrigger2DEvent);
        if (backUp != null) backUp.IsTriggered.AddValueListener<bool>(OnTrigger2DEvent);
        if (backDown != null) backDown.IsTriggered.AddValueListener<bool>(OnTrigger2DEvent);
    }

    private void OnTrigger2DEvent(bool triggered = false)
    {
        if (!CenterCollision)// && !FrontUpCollision && !BackDownCollision)
            GroundFlatness.Value = ((int)Flatness.NoGround);

        else if (FrontUpCollision || BackUpCollision)
            GroundFlatness.Value = ((int)Flatness.Hole);

        else if (FrontDownCollision && BackDownCollision)
            GroundFlatness.Value = ((int)Flatness.Flat);

        else if (!FrontDownCollision && !BackDownCollision)
            GroundFlatness.Value = ((int)Flatness.Point);

        else if (FrontDownCollision && !BackDownCollision)
            GroundFlatness.Value = ((int)Flatness.BackDrop);

        else if (!FrontDownCollision && BackDownCollision)
            GroundFlatness.Value = ((int)Flatness.FrontDrop);

        else GroundFlatness.Value = ((int)Flatness.Other);

        groundIsFlat.Value = (GroundFlatness == (int)Flatness.Flat);
    }
}