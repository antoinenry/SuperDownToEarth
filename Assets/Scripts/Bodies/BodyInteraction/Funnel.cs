using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Funnel : HidingPlace
{
    [Header("Travel")]
    public Funnel connectTo;
    [Min(0f)] public float travelStartDelay = .2f;
    [Min(0f)] public float travelDuration = .3f;
    [Min(0f)] public float velocityThreshold = 25f;
    
    private Coroutine currentTravelCoroutine;       

    protected override void OnBodyEnter(PhysicalBody body, float velocity)
    {
        Hide(body);        
        if (currentTravelCoroutine == null && velocity >= velocityThreshold)
            currentTravelCoroutine = StartCoroutine(TravelCoroutine());
    }

    protected override void OnPushOut()
    {
        if (currentTravelCoroutine != null)
        {
            StopCoroutine(currentTravelCoroutine);
            currentTravelCoroutine = null;
        }
        base.OnPushOut();
    }

    private IEnumerator TravelCoroutine()
    {
        if (travelStartDelay > 0f)
            yield return new WaitForSeconds(travelStartDelay);

        isHidingABody.Value = false;

        if (hiddenJumper != null)
        {
            if (jumpTriggersExit)
                hiddenJumper.tryJump.RemoveTriggerListener(OnPushOut);           

            hiddenJumper = null;
        }
        
        yield return new WaitForSeconds(travelDuration);
        
        connectTo.Hide(hiddenBody);
        hiddenBody = null;

        currentTravelCoroutine = null;
    }
}
