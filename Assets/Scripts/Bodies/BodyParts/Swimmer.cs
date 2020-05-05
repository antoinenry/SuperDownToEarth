using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Swimmer : BodyPart, IValueChangeEventsComponent
{
    public LayerMask floatLayer;

    public ValueChangeEvent IsInFluid = ValueChangeEvent.New<bool>();
    
    private int fluidCount;

    public void EnslaveValueChangeEvents(bool enslave)
    {
        IsInFluid.Enslave(enslave);
    }

    public int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        vces = new ValueChangeEvent[] { IsInFluid };
        return 1;
    }

    public int SetValueChangeEventsID()
    {
        IsInFluid.SetID("IsInFluid", this, 0);
        return 1;
    }

    private void Awake()
    {
        AttachedBody = GetComponent<Body>();
        fluidCount = 0;
    }

    private void FixedUpdate()
    {
        IsInFluid.Set(fluidCount > 0);
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
