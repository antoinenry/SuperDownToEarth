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

    public ValueChangeEvent IsWalking = ValueChangeEvent.New<bool>();

    private void Awake()
    {
        AttachedBody = GetComponent<Body>();
        Feet = GetComponent<Feet>();
        gearBox = GetComponent<GearBox>();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        if (gearBox != null)
        {
            gearBox.CurrentGear.AddListener<int>(OnSwitchGear);
            OnSwitchGear(gearBox.CurrentGear.Get<int>());
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();

        StopAllCoroutines();
        if (gearBox != null) gearBox.CurrentGear.RemoveListener<int>(OnSwitchGear);
    }

    private void FixedUpdate()
    {
        if (Feet.IsOnGround.Get<bool>() == true)
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
        if (Feet.IsOnGround.Get<bool>() == false || walkDirection == Direction.IDLE)
        {
            IsWalking.Set(false);
            currentWalkVelocity = Vector2.zero;
            CurrentDirection = Direction.IDLE;
            if (gearBox != null) gearBox.CurrentGear.Set(0);
        }
        else
            IsWalking.Set(true);        

        if (CurrentDirection != walkDirection)
        {
            CurrentDirection = walkDirection;
            if (walkDirection != Direction.IDLE)
                StartCoroutine(WalkCoroutine());
        }
    }

    private IEnumerator WalkCoroutine()
    {
        if (IsWalking.Get<bool>() == true)
        {
            if (CurrentDirection == Direction.RIGHT)
            {
                AttachedRigidbody.transform.localScale = Vector3.one;

                while (CurrentDirection == Direction.RIGHT && Feet.IsTumbling.Get<bool>() == false && AttachedRigidbody.simulated == true)
                {
                    currentWalkVelocity = Quaternion.Euler(0f, 0f, AttachedRigidbody.rotation) * Vector2.right * walkSpeed;
                    yield return new WaitForFixedUpdate();
                }
            }
            else if (CurrentDirection == Direction.LEFT)
            {
                AttachedRigidbody.transform.localScale = new Vector3(-1f, 1f, 1f);

                while (CurrentDirection == Direction.LEFT && Feet.IsTumbling.Get<bool>() == false && AttachedRigidbody.simulated == true)
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

        if (Feet.IsOnGround.Get<bool>() == false)
        {
            CurrentDirection = Direction.IDLE;
            currentWalkVelocity = Vector2.zero;
            IsWalking.Set(false);
            if (gearBox != null) gearBox.CurrentGear.Set(0);
        }
    }

    private void OnSwitchGear(int gear)
    {
        walkSpeed = gearBox.GetCurrentSpeed();
    }    
}
