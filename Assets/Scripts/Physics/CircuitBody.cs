using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CircuitBody : MonoBehaviour
{
    public Circuit circuit;
    public float speed;
    public bool invertDirection;
    public bool stepByStep;
    public bool mirror;

    public BoolChangeEvent enableMovement;
    public IntChangeEvent step;

    private Rigidbody2D rb;
    private int nextStep;
    private Vector2 nextPoint;
    private bool isMoving;

    [SerializeField] private bool _enableMovement = true;
    [SerializeField] private int _step;

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
            int currentStep = (int)step.Value;

            if (currentStep < 0) currentStep = circuit.Length - 1;
            else if (currentStep >= circuit.Length) currentStep = 0;

            step.Value = currentStep;
            transform.position = circuit.GetPoint(currentStep);
        }
    }
    
    private void OnEnable()
    {
        enableMovement.AddValueListener<bool>(OnEnableMovement);
        step.AddValueListener<int>(OnStepChange);
    }

    private void OnDisable()
    {
        enableMovement.RemoveValueListener<bool>(OnEnableMovement);
        step.RemoveValueListener<int>(OnStepChange);
    }

    private void OnEnableMovement(bool enable)
    {
        if (enable == true && isMoving == false)
            StartCoroutine(MoveCoroutine());
    }

    private void OnStepChange(int step)
    {
        if ((bool)enableMovement.Value == true && isMoving == false)
            StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        while((bool)enableMovement.Value)
        {
            isMoving = true;

            FixedUpdateMove(out int updateSteps);
            yield return new WaitForFixedUpdate();

            if (stepByStep == true && updateSteps > 0) enableMovement.Value = false;
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
        step.Value = nextStep;
        SetNextStep();
    }

    private void SetNextStep()
    {
        int stepDirection = invertDirection ? -1 : 1;
        int currentStep = (int)step.Value;

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
