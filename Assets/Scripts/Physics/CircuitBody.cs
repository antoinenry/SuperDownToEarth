using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Rigidbody2D))]
public class CircuitBody : MonoBehaviour
{
    public Circuit circuit;
    public float speed;
    public bool invertDirection;
    public bool stepByStep;
    public bool mirror;

    public BoolChangeEvent isMoving;
    public IntChangeEvent currentStep;

    private Rigidbody2D rb;
    private int nextStep;
    private Vector2 nextPoint;
    private Coroutine moveCoroutine;

    public void OnDrawGizmosSelected()
    {
        if (circuit != null) circuit.OnDrawGizmosSelected();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        SetNextStep();
    }

    private void Update()
    {
        if(Application.isPlaying == false && circuit != null)
        {
            if (currentStep < 0) currentStep.Value = circuit.Length - 1;
            else if (currentStep >= circuit.Length) currentStep.Value = 0;
            
            transform.position = circuit.GetPoint(currentStep);
        }
    }
    
    private void OnEnable()
    {
        isMoving.AddValueListener<bool>(OnMovement);
        currentStep.AddValueListener<int>(OnStepChange, isMoving);
    }

    private void OnDisable()
    {
        isMoving.RemoveValueListener<bool>(OnMovement);
        currentStep.RemoveValueListener<int>(OnStepChange);
    }

    private void OnMovement(bool startMoving)
    {
        if (startMoving == true)
        {
            if (moveCoroutine == null)
                moveCoroutine = StartCoroutine(MoveCoroutine());
        }        
        else if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }

    private void OnStepChange(int step)
    {
        isMoving.Value = true;
    }

    private IEnumerator MoveCoroutine()
    {
        while((bool)isMoving.Value)
        {
            FixedUpdateMove(out int updateSteps);
            yield return new WaitForFixedUpdate();

            if (stepByStep == true && updateSteps > 0) isMoving.Value = false;
        }

        rb.velocity = Vector2.zero;
        moveCoroutine = null;
    }

    private void FixedUpdateMove(out int numSteps)
    {
        numSteps = 0;

        Vector2 currentPosition = rb.position;
        float absSpeed = Mathf.Abs(speed);
        float distanceToGo = absSpeed * Time.fixedDeltaTime;
        float distanceToNextPoint = Vector2.Distance(currentPosition, nextPoint);

        if (stepByStep)
        {
            if (distanceToGo > distanceToNextPoint)
            {
                rb.MovePosition(nextPoint);
                rb.velocity = Vector2.zero;
                MoveOneStep();
                numSteps = 1;
                return;
            }
        }
        else
        {
            while (distanceToGo > distanceToNextPoint)
            {
                distanceToGo -= distanceToNextPoint;
                currentPosition = nextPoint;
                MoveOneStep();
                distanceToNextPoint = Vector2.Distance(currentPosition, nextPoint);
                numSteps++;
            }
        }        

        rb.velocity = absSpeed * (nextPoint - rb.position).normalized;
        return;
    }

    private void MoveOneStep()
    {
        currentStep.Value = nextStep;
        SetNextStep();
    }

    private void SetNextStep()
    {
        int stepDirection = invertDirection ? -1 : 1;
        int currentStep = (int)this.currentStep.Value;

        if (speed >= 0)
            nextStep = currentStep + stepDirection;
        else
            nextStep = currentStep - stepDirection;

        if (nextStep < 0)
        {
            if (mirror == false)
                nextStep += circuit.Length;
            else
            {
                nextStep = 1;
                invertDirection = !invertDirection;
            }
        }
        else if (nextStep >= circuit.Length)
        {
            if (mirror == false)
                nextStep -= circuit.Length;
            else
            {
                nextStep = circuit.Length - 2;
                invertDirection = !invertDirection;
            }
        }

        nextPoint = circuit.GetPoint(nextStep);
    }
}
