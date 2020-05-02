using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : BodyPart, IValueChangeEventsComponent
{
    public float walkSpeed;   
    
    public Feet Feet { get; private set; }

    public enum Direction { IDLE, RIGHT, LEFT }
    public Direction CurrentDirection { get; private set; }
    public int CurrentIntDirection { get => WalkDirectionToInt(CurrentDirection); }

    private Vector2 currentWalkVelocity;
    private GearBox gearBox;
    private bool switchingGears;

    public ValueChangeEvent IsWalking = ValueChangeEvent.New<bool>();

    public int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        vces = new ValueChangeEvent[] { IsWalking };
        return vces.Length;
    }

    public int SetValueChangeEventsID()
    {
        IsWalking.SetID("IsWalking", this, 0);
        return 1;
    }

    public void EnslaveValueChangeEvents(bool enslave)
    {
        IsWalking.Enslave(enslave);
    }

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
            gearBox.CurrentGear.AddListener<int>(OnSwitchGear);
            OnSwitchGear(gearBox.CurrentGear.GetValue<int>());
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (gearBox != null) gearBox.CurrentGear.RemoveListener<int>(OnSwitchGear);
    }

    private void FixedUpdate()
    {
        if (Feet.IsOnGround.GetValue<bool>() == true)
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
        if (Feet.IsOnGround.GetValue<bool>() == false || walkDirection == Direction.IDLE)
        {
            IsWalking.SetValue(false);
            currentWalkVelocity = Vector2.zero;
            CurrentDirection = Direction.IDLE;
            if (gearBox != null) gearBox.CurrentGear.SetValue(0);
        }
        else
            IsWalking.SetValue(true);        

        if (CurrentDirection != walkDirection)
        {
            CurrentDirection = walkDirection;
            if (walkDirection != Direction.IDLE)
                StartCoroutine(WalkCoroutine());
        }
    }

    private IEnumerator WalkCoroutine()
    {
        if (IsWalking.GetValue<bool>() == true)
        {
            if (CurrentDirection == Direction.RIGHT)
            {
                AttachedRigidbody.transform.localScale = Vector3.one;

                while (CurrentDirection == Direction.RIGHT && Feet.IsTumbling.GetValue<bool>() == false && AttachedRigidbody.simulated == true)
                {
                    currentWalkVelocity = Quaternion.Euler(0f, 0f, AttachedRigidbody.rotation) * Vector2.right * walkSpeed;
                    yield return new WaitForFixedUpdate();
                }
            }
            else if (CurrentDirection == Direction.LEFT)
            {
                AttachedRigidbody.transform.localScale = new Vector3(-1f, 1f, 1f);

                while (CurrentDirection == Direction.LEFT && Feet.IsTumbling.GetValue<bool>() == false && AttachedRigidbody.simulated == true)
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

        if (Feet.IsOnGround.GetValue<bool>() == false)
        {
            CurrentDirection = Direction.IDLE;
            currentWalkVelocity = Vector2.zero;
            IsWalking.SetValue(false);
            if (gearBox != null) gearBox.CurrentGear.SetValue(0);
        }
    }

    private void OnSwitchGear(int gear)
    {
        walkSpeed = gearBox.GetCurrentSpeed();
    }    
}
