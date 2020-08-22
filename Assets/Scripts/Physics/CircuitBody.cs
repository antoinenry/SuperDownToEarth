using System.Collections;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Rigidbody2D))]
public class CircuitBody : StepByStep
{
    public Circuit circuit;
    [Min(0f)] public float speed;
    [Min(0f)] public float stepRadius;

    private Rigidbody2D rb;    

    public void OnDrawGizmosSelected()
    {
        if (circuit != null) circuit.OnDrawGizmosSelected();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override int GetStepCount()
    {
        return circuit != null ? circuit.Length : 0;
    }

    public override void MoveTo(int destinationStep)
    {
        if (circuit == null) return;

        int correctDestinationStep = GetCorrectIndex(destinationStep);
        if (cycleType == CycleType.PingPong && correctDestinationStep != destinationStep)
            invertDirection = !invertDirection;

        if (Application.isPlaying == true)
        {
            moveToStep.Value = correctDestinationStep;
        }
        else
        {
            LastReachedStep = correctDestinationStep;
            transform.position = circuit.GetPosition(correctDestinationStep);
            moveToStep.SetValueWithoutTriggeringEvent(correctDestinationStep);
        }
    }

    protected override IEnumerator MoveCoroutine(int destinationStep)
    {
        while (destinationStep != LastReachedStep)
        {
            MoveFromPosition(rb.position, destinationStep, Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        moveCoroutine = null;

        if (autoMoving == true)
        {
            int nextStep = LastReachedStep + CurrentDirection;
            if (cycleType != CycleType.Single || (nextStep == GetCorrectIndex(nextStep)))
                MoveTo(nextStep);
            else
                autoMoving.SetValueWithoutTriggeringEvent(false);
        }

        moveCoroutine = null;
    }

    private bool MoveFromPosition(Vector2 fromPosition, int destinationStep, float deltaTime)
    {
        if (deltaTime <= 0f || speed <= 0f) return false;
        
        int correctDestinationStep = GetCorrectIndex(destinationStep);
        Vector2 destinationPoint = circuit.GetPosition(correctDestinationStep);
        if (Vector2.Distance(fromPosition, destinationPoint) <= stepRadius)
        {
            LastReachedStep = correctDestinationStep;
            moveToStep.SetValueWithoutTriggeringEvent(correctDestinationStep);
            return false;
        }

        if (cycleType == CycleType.PingPong && destinationStep != LastReachedStep)
        {
            invertDirection = correctDestinationStep < LastReachedStep;
        }
        
        float maxDistance = speed * deltaTime;
        int requestedNextStep = LastReachedStep + CurrentDirection;
        int correctNextStep = GetCorrectIndex(requestedNextStep);
        Vector2 nextStepPosition = circuit.GetPosition(correctNextStep);
        float distanceToNextStep = Vector2.Distance(fromPosition, nextStepPosition);   
        
        if (correctDestinationStep == correctNextStep)
        {
            if (distanceToNextStep < maxDistance)
            {
                rb.MovePosition(destinationPoint);
                rb.velocity = Vector2.zero;

                LastReachedStep = correctDestinationStep;
                moveToStep.SetValueWithoutTriggeringEvent(correctDestinationStep);
                return true;
            }
        }
        else
        {
            if (distanceToNextStep < maxDistance)
            {
                maxDistance -= distanceToNextStep;
                LastReachedStep = correctNextStep;
                return MoveFromPosition(nextStepPosition, destinationStep, deltaTime);
            }
        }        

        rb.velocity = speed * (nextStepPosition - rb.position).normalized;

        return true;
    }
}
