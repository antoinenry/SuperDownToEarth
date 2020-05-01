using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Vehicle : PhysicalBody, IValueChangeEventsComponent
{
    [Header("Vehicle")]
    public Transform seat;
    public Transform exit;
    public float exitForce;
    public float exitAnimationDelay;
    
    public ValueChangeEvent IsFull = ValueChangeEvent.NewValueChangeEvent<bool>();
    public Body BodyInside { get; private set; }
    
    public int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        vces = new ValueChangeEvent[] { IsFull };
        return vces.Length;
    }

    public void SetValueChangeEventsID()
    {
        IsFull.SetID("IsFull", this, 0);
    }

    public void EnslaveValueChangeEvents(bool enslave)
    {
        IsFull.Enslave<bool>(enslave);
    }

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
            IsFull.SetValue(false);
            isFree = true;
            pilotableConfig.Enabled = Pilotable.PartEnabled.AllDisabled;
        }
        else
        {
            IsFull.SetValue(true);
            isFree = false;
            pilotableConfig.Enabled = Pilotable.PartEnabled.AllEnabled;
        }
    }
}