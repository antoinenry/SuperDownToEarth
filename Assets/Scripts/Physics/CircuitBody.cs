using System.Collections;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Rigidbody2D))]
public class CircuitBody : MonoBehaviour
{
    public Circuit circuit;
    [Min(0f)] public float speed;
    [Min(0f)] public float stepRadius;
    public bool invertDirection;
    
    public IntChangeEvent moveToStep;
    public BoolChangeEvent autoMoving;
    public Trigger move;

    private Rigidbody2D rb;
    private Coroutine moveCoroutine;

    public int LastReachedStep { get; private set; }
    public int CurrentDirection { get => invertDirection ? -1 : 1; }
    public bool IsMoving { get => moveCoroutine != null; }

    public void OnDrawGizmosSelected()
    {
        if (circuit != null) circuit.OnDrawGizmosSelected();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void OnEnable()
    {
        if (moveToStep == null) moveToStep = new IntChangeEvent();
        if (autoMoving == null) autoMoving = new BoolChangeEvent();
        if (move == null) move = new Trigger();

        moveToStep.AddValueListener<int>(OnMoveStepChange);
        autoMoving.AddValueListener<bool>(OnAutoMove);
        move.AddTriggerListener(MoveOneStep);
    }

    private void OnDisable()
    {
        moveToStep.RemoveValueListener<int>(OnMoveStepChange);
        autoMoving.RemoveValueListener<bool>(OnAutoMove);
        move.RemoveTriggerListener(MoveOneStep);
    }

    private void OnMoveStepChange(int step)
    {
        if (Application.isPlaying)
        {
            if (IsMoving == true) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveCoroutine(step));
        }
        else
            MoveTo(step);
    }

    private void OnAutoMove(bool auto)
    {
        if (Application.isPlaying == true && auto && IsMoving == false)
            MoveOneStep();
    }

    private void MoveOneStep()
    {
        MoveTo(LastReachedStep + CurrentDirection);
    }

    private IEnumerator MoveCoroutine(int destinationStep)
    {
        while (destinationStep != LastReachedStep)
        {
            MoveTo(rb.position, destinationStep, Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        moveCoroutine = null;

        if (autoMoving == true)
        {
            int nextStep = LastReachedStep + CurrentDirection;
            if (circuit.cycleType != Circuit.CycleType.Single || (nextStep == circuit.GetCorrectPositionIndex(nextStep)))
                MoveTo(nextStep);
            else
                autoMoving.SetValueWithoutTriggeringEvent(false);
        }
    }

    private bool MoveTo(Vector2 fromPosition, int destinationStep, float deltaTime)
    {
        if (deltaTime <= 0f || speed <= 0f) return false;
        
        int correctDestinationStep = circuit.GetCorrectPositionIndex(destinationStep);
        Vector2 destinationPoint = circuit.GetPosition(correctDestinationStep);
        if (Vector2.Distance(fromPosition, destinationPoint) <= stepRadius)
        {
            LastReachedStep = correctDestinationStep;
            moveToStep.SetValueWithoutTriggeringEvent(correctDestinationStep);
            return false;
        }

        if (circuit.cycleType == Circuit.CycleType.PingPong && destinationStep != LastReachedStep)
        {
            invertDirection = correctDestinationStep < LastReachedStep;
        }
        
        float maxDistance = speed * deltaTime;
        int requestedNextStep = LastReachedStep + CurrentDirection;
        int correctNextStep = circuit.GetCorrectPositionIndex(requestedNextStep);
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
                return MoveTo(nextStepPosition, destinationStep, deltaTime);
            }
        }        

        rb.velocity = speed * (nextStepPosition - rb.position).normalized;
        return true;
    }    

    public void MoveTo(int destinationStep)
    {
        if (circuit == null) return;

        int correctDestinationStep = circuit.GetCorrectPositionIndex(destinationStep);
        if (circuit.cycleType == Circuit.CycleType.PingPong && correctDestinationStep != destinationStep)
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
}
