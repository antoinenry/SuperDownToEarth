﻿using System;
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
    
    public ValueChangeEvent IsFull = ValueChangeEvent.New<bool>();
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