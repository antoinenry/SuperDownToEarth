using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearBox : BodyPart, IEventsHubElement
{
    public float[] speed;
    [Min(0f)] public float switchDelay;

    public enum ExposedEvents { onGearUp, currentGear }
    public TriggerEvent OnGearUp = new TriggerEvent();
    public ValueChangeEvent<int> CurrentGear = new ValueChangeEvent<int>();

    private float lastSwitchTime;

    private void Awake()
    {
        //CurrentGear = new ValueChangeEvent<int>();
    }

    private void Start()
    {
        lastSwitchTime = Time.fixedTime;
    }

    public float GetCurrentSpeed()
    {  
        int speedIndex = ClampedGear(CurrentGear.Value);
        return speed[speedIndex];
    }

    public void GearUp()
    {
        float currentTime = Time.fixedTime;

        if(currentTime >= lastSwitchTime + switchDelay)
        {
            int upGear = ClampedGear(CurrentGear.Value + 1);
            if (upGear != CurrentGear.Value)
            {
                CurrentGear.Value = upGear;
                OnGearUp.Trigger();
                lastSwitchTime = currentTime;
            }
        }
    }

    public void GearDown()
    {
        float currentTime = Time.fixedTime;

        if (currentTime >= lastSwitchTime + switchDelay)
        {
            CurrentGear.Value = ClampedGear(CurrentGear.Value - 1);
            lastSwitchTime = currentTime;
        }
    }

    public int ClampedGear(int value, bool allowNegative = false)
    {
        if (allowNegative == false)
            return Mathf.Clamp(value, 0, speed.Length - 1);
        else
            return Mathf.Clamp(value, 1 - speed.Length, speed.Length - 1);
    }

    public bool GetValueChangeEvent(int index, out IValueChangeEvent iValueChangeEvent)
    {
        switch (index)
        {
            case (int)ExposedEvents.onGearUp:
                iValueChangeEvent = OnGearUp;
                return true;

            case (int)ExposedEvents.currentGear:
                iValueChangeEvent = CurrentGear;
                return true;
        }

        iValueChangeEvent = null;
        return false;
    }

    public void GetValueChangeEventsNamesAndTypes(out string[] names, out Type[] types)
    {
        names = Enum.GetNames(typeof(ExposedEvents));
        types = new Type[] { null, typeof(int) };
    }

    public int GetValueChangeEventIndex(string vceName)
    {
        List<string> names = new List<string>(Enum.GetNames(typeof(ExposedEvents)));
        return names.IndexOf(vceName);
    }
}
