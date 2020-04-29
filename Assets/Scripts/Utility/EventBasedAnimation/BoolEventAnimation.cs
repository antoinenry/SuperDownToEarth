using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class BoolEventAnimation : EventAnimation
{
    private UnityAction<bool> animationAction;
    private ValueChangeEvent<bool> boolEvent;

    public BoolEventAnimation(Animator animator, ValueChangeEvent<bool> bEvent, string animatorParameterName)
    {
        boolEvent = bEvent;
        animationAction = new UnityAction<bool>(boolValue => animator.SetBool(animatorParameterName, boolValue));
        Activate();
    }

    ~BoolEventAnimation()
    {
        Deactivate();
    }

    public override void Activate()
    {
        boolEvent.AddListener(animationAction);
    }

    public override void Deactivate()
    {
        boolEvent.RemoveListener(animationAction);
    }
}
