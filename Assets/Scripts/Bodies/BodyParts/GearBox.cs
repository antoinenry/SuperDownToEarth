using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearBox : BodyPart
{
    public float[] speed;
    [Min(0f)] public float switchDelay;

    public ValueChangeEvent CurrentGear = ValueChangeEvent.New<int>();
    public ValueChangeEvent OnGearUp = ValueChangeEvent.New<trigger>();
    public ValueChangeEvent OnGearDown = ValueChangeEvent.New<trigger>();

    private float timeSinceLastSwitch;

    public float GetCurrentSpeed()
    {  
        int speedIndex = ClampedGear(CurrentGear.Get<int>());
        return speed[speedIndex];
    }

    public void GearUp()
    {
        timeSinceLastSwitch += Time.fixedTime;
        if(timeSinceLastSwitch >= switchDelay)
        {
            CurrentGear.Set(CurrentGear.Get<int>() + 1);
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
            CurrentGear.Set(CurrentGear.Get<int>() - 1);
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
