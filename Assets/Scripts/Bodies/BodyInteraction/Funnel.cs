using System.Collections;
using UnityEngine;

public class Funnel : HidingPlace
{
    [Header("Travel")]
    public Funnel connectTo;
    [Min(0f)] public float travelStartDelay = .2f;
    [Min(0f)] public float travelDuration = .3f;
    
    private Coroutine currentTravelCoroutine;

    public override void Hide(Hider target)
    {
        if (target != null)
        {
            base.Hide(target);
            target.tryFunnel.AddTriggerListener(OnHiderTravel);
        }
    }

    protected override void OnPushOut()
    {
        if (currentTravelCoroutine != null)
        {
            StopCoroutine(currentTravelCoroutine);
            currentTravelCoroutine = null;
        }

        if(hider != null)
        {
            hider.tryFunnel.RemoveTriggerListener(OnHiderTravel);
            base.OnPushOut();
        }
    }

    private void OnHiderTravel()
    {
        StartCoroutine(TravelCoroutine());
    }

    private IEnumerator TravelCoroutine()
    {
        if (hider != null)
            hider.tryFunnel.RemoveTriggerListener(OnHiderTravel);


        if (travelStartDelay > 0f)
        yield return new WaitForSeconds(travelStartDelay);

        Hider traveler = hider;
        if (hider != null)
        {
            hider.stopHiding.RemoveTriggerListener(PushOut);
            hider = null;
        }
        isHidingABody.Value = false;
        
        yield return new WaitForSeconds(travelDuration);
        
        connectTo.Hide(traveler);

        currentTravelCoroutine = null;
    }
}
