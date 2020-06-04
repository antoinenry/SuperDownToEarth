using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : BodyPart
{
    public float walkSpeed;   
    
    public Feet Feet { get; private set; }

    public enum Direction { IDLE, RIGHT, LEFT }
    public Direction CurrentDirection { get; private set; }
    public int CurrentIntDirection { get => WalkDirectionToInt(CurrentDirection); }

    private Vector2 currentWalkVelocity;
    private GearBox gearBox;
    private bool switchingGears;

    public BoolChangeEvent IsWalking;

    private void Awake()
    {
        AttachedBody = GetComponent<Body>();
        Feet = GetComponent<Feet>();
        gearBox = GetComponent<GearBox>();
    }

    private void OnEnable()
    {
        if (gearBox != null)
        {
            gearBox.CurrentGear.AddValueListener<int>(OnSwitchGear);
            OnSwitchGear((int)gearBox.CurrentGear.Value);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (gearBox != null) gearBox.CurrentGear.RemoveValueListener<int>(OnSwitchGear);
    }

    private void FixedUpdate()
    {
        if ((bool)Feet.IsOnGround.Value == true)
            AttachedRigidbody.velocity = Feet.GroundVelocity + currentWalkVelocity;
    }

    public static Direction IntToWalkDirection(int intDirection)
    {
        if (intDirection > 0 ) return Walker.Direction.RIGHT;
        else if (intDirection < 0) return Walker.Direction.LEFT;
        else return Walker.Direction.IDLE;
    }

    public static int WalkDirectionToInt(Walker.Direction walkDirection)
    {
        switch (walkDirection)
        {
            case Walker.Direction.RIGHT: return 1;
            case Walker.Direction.LEFT: return - 1;
        }

        return 0;
    }

    public void Walk(int intDirection)
    {
        Walk(IntToWalkDirection(intDirection));
    }

    public void Walk(Direction walkDirection)
    {
        if ((bool)Feet.IsOnGround.Value == false || walkDirection == Direction.IDLE)
        {
            IsWalking.Value = false;
            currentWalkVelocity = Vector2.zero;
            CurrentDirection = Direction.IDLE;
            if (gearBox != null) gearBox.CurrentGear.Value = 0;
        }
        else
            IsWalking.Value = true;        

        if (CurrentDirection != walkDirection)
        {
            CurrentDirection = walkDirection;
            if (walkDirection != Direction.IDLE)
                StartCoroutine(WalkCoroutine());
        }
    }

    private IEnumerator WalkCoroutine()
    {
        if ((bool)IsWalking.Value == true)
        {
            if (CurrentDirection == Direction.RIGHT)
            {
                AttachedRigidbody.transform.localScale = Vector3.one;

                while (CurrentDirection == Direction.RIGHT && (bool)Feet.IsTumbling.Value== false && AttachedRigidbody.simulated == true)
                {
                    currentWalkVelocity = Quaternion.Euler(0f, 0f, AttachedRigidbody.rotation) * Vector2.right * walkSpeed;
                    yield return new WaitForFixedUpdate();
                }
            }
            else if (CurrentDirection == Direction.LEFT)
            {
                AttachedRigidbody.transform.localScale = new Vector3(-1f, 1f, 1f);

                while (CurrentDirection == Direction.LEFT && (bool)Feet.IsTumbling.Value == false && AttachedRigidbody.simulated == true)
                {
                    currentWalkVelocity = Quaternion.Euler(0f, 0f, AttachedRigidbody.rotation) * Vector2.left * walkSpeed;
                    yield return new WaitForFixedUpdate();
                }
            }
            else
            {
                currentWalkVelocity = Vector2.zero;
            }
        }

        if ((bool)Feet.IsOnGround.Value == false)
        {
            CurrentDirection = Direction.IDLE;
            currentWalkVelocity = Vector2.zero;
            IsWalking.Value = false;
            if (gearBox != null) gearBox.CurrentGear.Value = 0;
        }
    }

    private void OnSwitchGear(int gear)
    {
        walkSpeed = gearBox.GetCurrentSpeed();
    }    
}
