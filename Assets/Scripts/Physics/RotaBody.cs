using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RotaBody : StepByStep
{
    public float speed;
    public float[] steps;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override int GetStepCount()
    {
        return steps != null ? steps.Length : 0;
    }

    protected override void OnAutoMove(bool auto)
    {
        if (Application.isPlaying == true)
        {
            if (auto && IsMoving == false)
                MoveOneStep();
            else
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
                rb.angularVelocity = 0f;
            }
        }
    }

    public override void MoveTo(int destinationStep)
    {
        int correctDestinationStep = GetCorrectIndex(destinationStep);

        if (StepCount == 0)
        {
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveCoroutine(-1));
        }
        else
        {
            if (cycleType == CycleType.PingPong && correctDestinationStep != destinationStep)
                invertDirection = !invertDirection;

            if (Application.isPlaying == true)
            {
                moveToStep.Value = correctDestinationStep;
            }
            else
            {
                LastReachedStep = correctDestinationStep;
                transform.rotation = Quaternion.Euler(0f, 0f, steps[correctDestinationStep]);
                moveToStep.SetValueWithoutTriggeringEvent(correctDestinationStep);
            }
        }
    }

    protected override IEnumerator MoveCoroutine(int destinationStep)
    {
        if (StepCount == 0)
        {
            while(autoMoving == true)
            {
                rb.angularVelocity = invertDirection ? -speed : speed;
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            while (destinationStep != LastReachedStep)
            {
                MoveFromRotation(rb.rotation, destinationStep, Time.fixedDeltaTime);
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
        }

        moveCoroutine = null;
    }

    private bool MoveFromRotation(float fromRotation, int destinationStep, float deltaTime)
    {
        if (deltaTime <= 0f || speed <= 0f) return false;

        int correctDestinationStep = GetCorrectIndex(destinationStep);
        float destinationAngle = steps[correctDestinationStep];

        if (Mathf.Abs(Mathf.DeltaAngle(fromRotation, destinationAngle)) < Mathf.Abs(speed * deltaTime))
        {
            LastReachedStep = correctDestinationStep;
            moveToStep.SetValueWithoutTriggeringEvent(correctDestinationStep);
            return false;
        }

        if (cycleType == CycleType.PingPong && destinationStep != LastReachedStep)
        {
            invertDirection = correctDestinationStep < LastReachedStep;
        }

        float maxRotation = speed * deltaTime;
        int requestedNextStep = LastReachedStep + CurrentDirection;
        int correctNextStep = GetCorrectIndex(requestedNextStep);
        float nextStepRotation = steps[correctNextStep];
        float rotationToNextStep = Mathf.DeltaAngle(fromRotation, nextStepRotation);

        if (correctDestinationStep == correctNextStep)
        {
            if (rotationToNextStep < maxRotation)
            {
                rb.MoveRotation(destinationAngle);
                rb.angularVelocity = 0f;

                LastReachedStep = correctDestinationStep;
                moveToStep.SetValueWithoutTriggeringEvent(correctDestinationStep);
                return true;
            }
        }
        else
        {
            if (rotationToNextStep < maxRotation)
            {
                maxRotation -= rotationToNextStep;
                LastReachedStep = correctNextStep;
                return MoveFromRotation(nextStepRotation, destinationStep, deltaTime);
            }
        }

        rb.angularVelocity = speed * Mathf.Sign(rotationToNextStep);

        return true;
    }
}
