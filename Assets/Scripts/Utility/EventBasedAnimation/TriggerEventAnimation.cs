using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class TriggerEventAnimation : EventAnimation
{
    private UnityAction animationAction;
    private TriggerEvent triggerEvent;
    
    public TriggerEventAnimation(Animator animator, TriggerEvent tEvent, string animatorParameterName)
    {
        triggerEvent = tEvent;
        animationAction = new UnityAction(() => animator.SetTrigger(animatorParameterName));
        Activate();
    }

    ~TriggerEventAnimation()
    {
        Deactivate();
    }

    public override void Activate()
    {
        triggerEvent.AddListener(animationAction);
    }

    public override void Deactivate()
    {
        triggerEvent.RemoveListener(animationAction);
    }    
}
