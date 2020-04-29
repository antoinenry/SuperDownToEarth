using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class IntEventAnimation : EventAnimation
{
    private UnityAction<int> animationAction;
    private ValueChangeEvent<int> intEvent;

    public IntEventAnimation(Animator animator, ValueChangeEvent<int> bEvent, string animatorParameterName)
    {
        intEvent = bEvent;
        animationAction = new UnityAction<int>(intValue => animator.SetInteger(animatorParameterName, intValue));
        Activate();
    }

    ~IntEventAnimation()
    {
        Deactivate();
    }

    public override void Activate()
    {
        intEvent.AddListener(animationAction);
    }

    public override void Deactivate()
    {
        intEvent.RemoveListener(animationAction);
    }
}
