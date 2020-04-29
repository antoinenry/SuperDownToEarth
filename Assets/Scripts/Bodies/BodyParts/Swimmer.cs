﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Swimmer : BodyPart
{
    public LayerMask floatLayer;

    public ValueChangeEvent<bool> IsInFluid;
    
    private int fluidCount;

    private void Awake()
    {
        AttachedBody = GetComponent<Body>();
        IsInFluid = new ValueChangeEvent<bool>();
        fluidCount = 0;
    }

    private void FixedUpdate()
    {
        IsInFluid.Value = (fluidCount > 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & floatLayer) != 0)
            fluidCount++;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & floatLayer) != 0)
            fluidCount--;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //sink stuffs
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //sink stuffs
    }
}
