using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : BodyPart
{
    public float spinVelocity;
    public float spinInertia;
    public float tumbleRecuperation;
    
    public Feet Feet { get; private set; }

    public enum Direction { IDLE, CLOCKWISE, COUNTERCLOCKWISE }
    public Direction CurrentDirection { get; private set; }

    private void Awake()
    {
        AttachedBody = GetComponent<Body>();
        Feet = GetComponent<Feet>();
    }

    private void SetSpinVelocity(float sv)
    {
        if (spinInertia <= 0f)
            AttachedRigidbody.angularVelocity = sv;
        else
            AttachedRigidbody.angularVelocity = Mathf.MoveTowards(AttachedRigidbody.angularVelocity, sv, Time.fixedDeltaTime / spinInertia);
    }

    public Direction IntToSpinDirection(int intDirection)
    {
        switch (intDirection)
        {
            case 1: return Spinner.Direction.CLOCKWISE;
            case -1: return Spinner.Direction.COUNTERCLOCKWISE;
        }

        return Spinner.Direction.IDLE;
    }

    public void Spin(int intDirection)
    {
        Spin(IntToSpinDirection(intDirection));
    }

    public void Spin(Direction spinDirection)
    {
        if (Feet.IsOnGround.Get<bool>() == false && CurrentDirection != spinDirection)
        {
            CurrentDirection = spinDirection;
            StartCoroutine(SpinCoroutine());
        }
    }

    private IEnumerator SpinCoroutine()
    {
        switch (CurrentDirection)
        {
            case Direction.IDLE:
                while (CurrentDirection == Direction.IDLE && Feet.IsOnGround.Get<bool>() == false && Feet.IsTumbling.Get<bool>() == false)
                {
                    SetSpinVelocity(0f);
                    yield return new WaitForFixedUpdate();
                }
                break;

            case Direction.CLOCKWISE:
                while (CurrentDirection == Direction.CLOCKWISE && Feet.IsOnGround.Get<bool>() == false && Feet.IsTumbling.Get<bool>() == false)
                {
                    SetSpinVelocity(-spinVelocity);
                    yield return new WaitForFixedUpdate();
                }
                break;

            case Direction.COUNTERCLOCKWISE:
                while (CurrentDirection == Direction.COUNTERCLOCKWISE && Feet.IsOnGround.Get<bool>() == false && Feet.IsTumbling.Get<bool>() == false)
                {
                    SetSpinVelocity(spinVelocity);
                    yield return new WaitForFixedUpdate();
                }
                break;
        }

        CurrentDirection = Direction.IDLE;
    }
}
