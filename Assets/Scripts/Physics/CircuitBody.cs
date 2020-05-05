using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CircuitBody : MonoBehaviour, IValueChangeEventsComponent
{
    public bool enableMovement = true;
    public Circuit circuit;
    public float speed;
    public bool invertDirection;
    public bool stepByStep;
    public bool mirror;
    
    private Rigidbody2D rb;
    private int nextStep;
    private Vector2 nextPoint;
    private bool isMoving;

    public ValueChangeEvent Step = ValueChangeEvent.New<int>();

    public int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        vces = new ValueChangeEvent[] { Step };
        return vces.Length;
    }

    public int SetValueChangeEventsID()
    {
        Step.SetID("Step", this, 0);
        return 1;
    }

    public void EnslaveValueChangeEvents(bool enslave)
    {
        Step.Enslave(enslave);
    }

    public void OnDrawGizmosSelected()
    {
        if (circuit != null) circuit.OnDrawGizmosSelected();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(Application.isPlaying == false && circuit != null)
        {
            int currentStep = Step.Get<int>();

            if (currentStep < 0) currentStep = circuit.Length - 1;
            else if (currentStep >= circuit.Length) currentStep = 0;

            Step.Set<int>(currentStep);
            transform.position = circuit.GetPoint(currentStep);
        }
    }

    private void OnEnable()
    {
        Step.AddListener<int>(OnStepChange);
    }

    private void OnDisable()
    {
        Step.RemoveListener<int>(OnStepChange);
    }

    private void OnStepChange(int step)
    {
        if (enableMovement == true && isMoving == false)
            StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        while(enableMovement)
        {
            isMoving = true;

            FixedUpdateMove(out int updateSteps);
            yield return new WaitForFixedUpdate();

            if (stepByStep == true && updateSteps > 0) enableMovement = false;
        }

        rb.velocity = Vector2.zero;
        isMoving = false;
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
        Step.Set<int>(nextStep);
        SetNextStep();
    }

    private void SetNextStep()
    {
        int stepDirection = invertDirection ? -1 : 1;
        int currentStep = Step.Get<int>();

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
