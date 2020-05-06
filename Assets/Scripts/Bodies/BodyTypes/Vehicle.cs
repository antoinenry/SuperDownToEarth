using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Vehicle : PhysicalBody //, IValueChangeEventsComponent
{
    [Header("Vehicle")]
    public Transform seat;
    public Transform exit;
    public float exitForce;
    public float exitAnimationDelay;
    
    public ValueChangeEvent IsFull = ValueChangeEvent.New<bool>();
    public Body BodyInside { get; private set; }
    
    public override int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        ValueChangeEvent[] thisEvents = new ValueChangeEvent[] { IsFull };
        int thisEventsCount = thisEvents.Length;
        int baseEventsCount = base.GetValueChangeEvents(out vces);

        Array.Resize(ref vces, baseEventsCount + thisEventsCount);
        thisEvents.CopyTo(vces, baseEventsCount);
        
        return vces.Length;
    }

    public override int SetValueChangeEventsID()
    {
        int idCount = base.SetValueChangeEventsID();
        IsFull.SetID("IsFull", this, idCount++);
        return idCount;
    }

    public override void EnslaveValueChangeEvents(bool enslave)
    {
        base.EnslaveValueChangeEvents(enslave);
        IsFull.Enslave(enslave);
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
            IsFull.Set(false);
            isFree = true;
        }
        else
        {
            IsFull.Set(true);
            isFree = false;
        }
    }
}