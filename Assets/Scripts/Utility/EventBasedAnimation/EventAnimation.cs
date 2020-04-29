using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public abstract class EventAnimation
{   
    public static EventAnimation NewEventAnimation(Animator animator, IValueChangeEvent vce, string animatorParameterName)
    {
        if (vce.GetValueType() == null)
            return new TriggerEventAnimation(animator, vce as TriggerEvent, animatorParameterName);

        if (vce.GetValueType() == typeof(bool))
            return new BoolEventAnimation(animator, vce as ValueChangeEvent<bool>, animatorParameterName);

        if (vce.GetValueType() == typeof(int))
            return new IntEventAnimation(animator, vce as ValueChangeEvent<int>, animatorParameterName);

        if (vce.GetValueType() == typeof(float))
            return new FloatEventAnimation(animator, vce as ValueChangeEvent<float>, animatorParameterName);

        return null;
    }

    public abstract void Activate();
    public abstract void Deactivate();
}
