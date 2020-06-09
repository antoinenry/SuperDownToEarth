using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Vehicle : PhysicalBody
{
    [Header("Vehicle")]
    public Transform seat;
    public Transform exit;
    public float exitForce;
    public float exitAnimationDelay;

    public Trigger eject;

    public BoolChangeEvent IsFull;
    public Body BodyInside { get; private set; }

    private void Awake()
    {
        SetBodyInside(BodyInside);
    }

    private void Start()
    {
        eject.AddTriggerListener(Eject);
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
        }
        else
        {
            IsFull.Value = true;
            isFree = false;
        }
    }
}