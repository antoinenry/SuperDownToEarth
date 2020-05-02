using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearBox : BodyPart, IValueChangeEventsComponent
{
    public float[] speed;
    [Min(0f)] public float switchDelay;

    public ValueChangeEvent CurrentGear = ValueChangeEvent.New<int>();
    public ValueChangeEvent OnGearUp = ValueChangeEvent.New<trigger>();
    public ValueChangeEvent OnGearDown = ValueChangeEvent.New<trigger>();

    private float timeSinceLastSwitch;
    
    public int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        vces = new ValueChangeEvent[] { CurrentGear, OnGearUp, OnGearDown };
        return vces.Length;
    }

    public int SetValueChangeEventsID()
    {
        CurrentGear.SetID("CurrentGear", this, 0);
        OnGearUp.SetID("OnGearUp", this, 1);
        OnGearDown.SetID("OnGearDown", this, 2);
        return 3;
    }

    public void EnslaveValueChangeEvents(bool enslave)
    {
        CurrentGear.Enslave(enslave);
        OnGearUp.Enslave(enslave);
        OnGearDown.Enslave(enslave);
    }

    public float GetCurrentSpeed()
    {  
        int speedIndex = ClampedGear(CurrentGear.GetValue<int>());
        return speed[speedIndex];
    }

    public void GearUp()
    {
        timeSinceLastSwitch += Time.fixedTime;
        if(timeSinceLastSwitch >= switchDelay)
        {
            CurrentGear.SetValue(CurrentGear.GetValue<int>() + 1);
            if (false)// (CurrentGear.Invoked)
            {
                OnGearUp.Invoke();
                timeSinceLastSwitch = 0f;
            }
        }
    }

    public void GearDown()
    {
        timeSinceLastSwitch += Time.fixedTime;
        if (timeSinceLastSwitch >= switchDelay)
        {
            CurrentGear.SetValue(CurrentGear.GetValue<int>() - 1);
            if (false)// (CurrentGear.Invoked)
            {
                OnGearUp.Invoke();
                timeSinceLastSwitch = 0f;
            }
        }
    }

    public int ClampedGear(int value, bool allowNegative = false)
    {
        if (allowNegative == false)
            return Mathf.Clamp(value, 0, speed.Length - 1);
        else
            return Mathf.Clamp(value, 1 - speed.Length, speed.Length - 1);
    }
}
