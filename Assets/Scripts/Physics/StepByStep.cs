using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StepByStep : MonoBehaviour
{
    public enum CycleType { Single, Loop, PingPong }

    public CycleType cycleType;
    public bool invertDirection;

    public IntChangeEvent moveToStep;
    public BoolChangeEvent autoMoving;
    public Trigger move;    

    protected Coroutine moveCoroutine;

    public int StepCount => GetStepCount();
    public int LastReachedStep { get; protected set; }
    public int CurrentDirection { get => invertDirection ? -1 : 1; }
    public bool IsMoving { get => moveCoroutine != null; }

    protected virtual void OnEnable()
    {
        moveToStep.AddValueListener<int>(OnMoveStepChange);
        autoMoving.AddValueListener<bool>(OnAutoMove, false);
        move.AddTriggerListener(MoveOneStep, false);
    }

    protected virtual void OnDisable()
    {
        moveToStep.RemoveValueListener<int>(OnMoveStepChange);
        autoMoving.RemoveValueListener<bool>(OnAutoMove);
        move.RemoveTriggerListener(MoveOneStep);

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }

    public abstract int GetStepCount();

    public int GetCorrectIndex(int index)
    {
        int correctIndex = -1;
        if (StepCount != 0)
        {
            if (StepCount == 1)
            {
                correctIndex = 0;
            }
            else
            {
                switch (cycleType)
                {
                    case CycleType.Single:
                        correctIndex = Mathf.Clamp(index, 0, StepCount - 1);
                        break;
                    case CycleType.Loop:
                        correctIndex = (int)Mathf.Repeat(index, StepCount);
                        break;
                    case CycleType.PingPong:
                        correctIndex = (int)Mathf.PingPong(index, StepCount - 1);
                        break;
                }
            }
        }

        return correctIndex;
    }

    protected virtual void OnMoveStepChange(int step)
    {
        if (Application.isPlaying)
        {
            if (IsMoving == true) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveCoroutine(step));
        }
        else
            MoveTo(step);
    }

    protected virtual void OnAutoMove(bool auto)
    {
        if (Application.isPlaying == true && auto && IsMoving == false)
            MoveOneStep();
    }

    protected virtual void MoveOneStep()
    {
        MoveTo(LastReachedStep + CurrentDirection);
    }

    public abstract void MoveTo(int destinationStep);

    protected abstract IEnumerator MoveCoroutine(int destinationStep);
}
