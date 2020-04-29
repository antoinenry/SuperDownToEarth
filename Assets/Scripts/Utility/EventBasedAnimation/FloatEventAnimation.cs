using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class FloatEventAnimation : EventAnimation
{
    private UnityAction<float> animationAction;
    private ValueChangeEvent<float> floatEvent;

    public FloatEventAnimation(Animator animator, ValueChangeEvent<float> bEvent, string animatorParameterName)
    {
        floatEvent = bEvent;
        animationAction = new UnityAction<float>(floatValue => animator.SetFloat(animatorParameterName, floatValue));
        Activate();
    }

    ~FloatEventAnimation()
    {
        Deactivate();
    }

    public override void Activate()
    {
        floatEvent.AddListener(animationAction);
    }

    public override void Deactivate()
    {
        floatEvent.RemoveListener(animationAction);
    }
}
