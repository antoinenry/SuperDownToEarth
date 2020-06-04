using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearBox : BodyPart
{
    public float[] speed;
    [Min(0f)] public float switchDelay;

    public IntChangeEvent CurrentGear;
    public Trigger OnGearUp;
    public Trigger OnGearDown;

    private float timeSinceLastSwitch;

    public float GetCurrentSpeed()
    {  
        int speedIndex = ClampedGear((int)CurrentGear.Value);
        return speed[speedIndex];
    }

    public void GearUp()
    {
        timeSinceLastSwitch += Time.fixedTime;
        if(timeSinceLastSwitch >= switchDelay)
        {
            CurrentGear.Value = (int)CurrentGear.Value + 1;
            /*
            if (CurrentGear.Invoked)
            {
                OnGearUp.Invoke();
                timeSinceLastSwitch = 0f;
            }*/
        }
    }

    public void GearDown()
    {
        timeSinceLastSwitch += Time.fixedTime;
        if (timeSinceLastSwitch >= switchDelay)
        {
            CurrentGear.Value = (int)CurrentGear.Value - 1;
            /*
            if (CurrentGear.Invoked)
            {
                OnGearUp.Invoke();
                timeSinceLastSwitch = 0f;
            }*/
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
