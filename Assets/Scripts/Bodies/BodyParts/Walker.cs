using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : BodyPart, IEventsHubElement
{
    public float walkSpeed;

    public enum ExposedEvents { isWalking }
    public ValueChangeEvent<bool> IsWalking = new ValueChangeEvent<bool>();
    
    public Feet Feet { get; private set; }

    public enum Direction { IDLE, RIGHT, LEFT }
    public Direction CurrentDirection { get; private set; }
    public int CurrentIntDirection { get => WalkDirectionToInt(CurrentDirection); }

    private Vector2 currentWalkVelocity;
    private GearBox gearBox;
    private bool switchingGears;

    private void Awake()
    {
        AttachedBody = GetComponent<Body>();
        Feet = GetComponent<Feet>();
        gearBox = GetComponent<GearBox>();

        //IsWalking = new ValueChangeEvent<bool>();
    }

    private void OnEnable()
    {
        if (gearBox != null)
        {
            gearBox.CurrentGear.AddListener(OnSwitchGear);
            OnSwitchGear(gearBox.CurrentGear.Value);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (gearBox != null) gearBox.CurrentGear.RemoveListener(OnSwitchGear);
    }

    private void FixedUpdate()
    {
        if (Feet.IsOnGround.Value == true)
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
        if (Feet.IsOnGround.Value == false || walkDirection == Direction.IDLE)
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
        if (IsWalking.Value == true)
        {
            if (CurrentDirection == Direction.RIGHT)
            {
                AttachedRigidbody.transform.localScale = Vector3.one;

                while (CurrentDirection == Direction.RIGHT && Feet.IsTumbling.Value == false && AttachedRigidbody.simulated == true)
                {
                    currentWalkVelocity = Quaternion.Euler(0f, 0f, AttachedRigidbody.rotation) * Vector2.right * walkSpeed;
                    yield return new WaitForFixedUpdate();
                }
            }
            else if (CurrentDirection == Direction.LEFT)
            {
                AttachedRigidbody.transform.localScale = new Vector3(-1f, 1f, 1f);

                while (CurrentDirection == Direction.LEFT && Feet.IsTumbling.Value == false && AttachedRigidbody.simulated == true)
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

        if (Feet.IsOnGround.Value == false)
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

    public bool GetValueChangeEvent(int index, out IValueChangeEvent iValueChangeEvent)
    {
        switch (index)
        {
            case (int)ExposedEvents.isWalking:
                iValueChangeEvent = IsWalking;
                return true;
        }

        iValueChangeEvent = null;
        return false;
    }

    public void GetValueChangeEventsNamesAndTypes(out string[] names, out Type[] types)
    {
        names = Enum.GetNames(typeof(ExposedEvents));
        types = new Type[] { typeof(bool) };
    }

    public int GetValueChangeEventIndex(string vceName)
    {
        List<string> names = new List<string>(Enum.GetNames(typeof(ExposedEvents)));
        return names.IndexOf(vceName);
    }
}
