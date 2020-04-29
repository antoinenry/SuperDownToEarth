using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CircuitBody : MonoBehaviour, IEventsHubElement
{
    public bool enableMovement = true;
    public Circuit circuit;
    public float speed;
    public bool invertDirection;
    public int currentStep;
    public bool stepByStep;
    public bool mirror;

    public enum ExposedEvents { step }
    public ValueChangeEvent<int> Step = new ValueChangeEvent<int>();

    private Rigidbody2D rb;
    private int nextStep;
    private Vector2 nextPoint;
    private bool isMoving;

    public void OnDrawGizmosSelected()
    {
        if (circuit != null) circuit.OnDrawGizmosSelected();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (circuit != null)
        {
            rb.position = circuit.GetPoint(currentStep);
            SetNextStep();
        }
    }

    private void Update()
    {
        if(Application.isPlaying == false && circuit != null)
        {
            if (currentStep < 0) currentStep = circuit.Length - 1;
            else if (currentStep >= circuit.Length) currentStep = 0;
            transform.position = circuit.GetPoint(currentStep);
        }
        else
        {
            if (Step.hasChanged)
            {
                if (Step.Value != currentStep)
                {
                    currentStep = Step.Value;
                    enableMovement = true;                    
                }

                Step.hasChanged = false;
            }
            else
                Step.Value = currentStep;

            if (enableMovement == true && isMoving == false)
                StartCoroutine(MoveCoroutine());
        }
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
        currentStep = nextStep;
        SetNextStep();
    }

    private void SetNextStep()
    {
        int stepDirection = invertDirection ? -1 : 1;

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

    public bool GetValueChangeEvent(int index, out IValueChangeEvent iValueChangeEvent)
    {
        switch (index)
        {
            case (int)ExposedEvents.step:
                iValueChangeEvent = Step;
                return true;
        }

        iValueChangeEvent = null;
        return false;
    }

    public void GetValueChangeEventsNamesAndTypes(out string[] names, out Type[] types)
    {
        names = Enum.GetNames(typeof(ExposedEvents));
        types = new Type[] { typeof(int) };
    }

    public int GetValueChangeEventIndex(string vceName)
    {
        List<string> names = new List<string>(Enum.GetNames(typeof(ExposedEvents)));
        return names.IndexOf(vceName);
    }
}
